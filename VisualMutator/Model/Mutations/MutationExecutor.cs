namespace VisualMutator.Model.Mutations
{
    using CSharpSourceEmitter;
    using Decompilation;
    using Exceptions;
    using Extensibility;
    using LinqLib.Sequence;
    using log4net;
    using Microsoft.Cci;
    using MutantsTree;
    using Operators;
    using StoringMutants;
    using Strilanc.Value;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Traversal;
    using Types;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    public interface IMutationExecutor
    {
        MultiDictionary<IMutationOperator, MutationTarget> FindTargets(CciModuleSource module, ProgressCounter subProgress);
        Task<MutationResult> ExecuteMutation(Mutant mutant, CciModuleSource moduleSource);
        Task<MutationResult> ExecuteMutation(Mutant mutant, List<CciModuleSource> moduleSource);
        Task<MutationResult> ExecuteMutation(Mutant mutant, CciModuleSource moduleSource, CciModuleSource moduleSource2);
    }

    public class MutationExecutor : IMutationExecutor
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CommonServices _svc;
        private readonly IOperatorUtils _operatorUtils;
        private readonly OptionsModel _options;
        private readonly MutationFilter _filter;
        private readonly IList<IMutationOperator> _mutOperators;
        private MultiDictionary<IMutationOperator, MutationTarget> _sharedTargets;

        public MutationExecutor(
            OptionsModel options,
        MutationSessionChoices choices, 
        CommonServices svc
        )
        {
            _svc = svc;
            _operatorUtils = new OperatorUtils();
            _options = options;
            _filter = choices.Filter;
            _mutOperators = choices.SelectedOperators;
            _sharedTargets = new MultiDictionary<IMutationOperator, MutationTarget>();
        }
       

        private IList<MutationTarget> LimitMutationTargets(IEnumerable<MutationTarget> targets)
        {
            // return targets.ToList();
            var mapping = targets.Shuffle()
                                 //  .SelectMany(pair => pair.Item2.Select(t => Tuple.Create(pair.Item1, t))).Shuffle()
                .Take(_options.MaxNumerOfMutantPerOperator).ToList();
            return mapping;
        }


        public MultiDictionary<IMutationOperator, MutationTarget> FindTargets(CciModuleSource module, ProgressCounter subProgress)
        {
            _log.Info("Finding targets for module: " + module.Module.Name);
            _log.Info("Using mutation operators: " + _mutOperators.Select(_ => _.Info.Id)
                .MayAggregate((a, b) => a + "," + b).Else("None"));

            var mergedTargets = new MultiDictionary<IMutationOperator, MutationTarget>();
            _sharedTargets = new MultiDictionary<IMutationOperator, MutationTarget>();
            subProgress.Initialize(_mutOperators.Count);
            foreach (var mutationOperator in _mutOperators)
            {
                try
                {
                    subProgress.Progress();
                    var ded = mutationOperator.CreateVisitor();
                    IOperatorCodeVisitor operatorVisitor = ded;
                    operatorVisitor.Host = module.Host;
                    operatorVisitor.OperatorUtils = _operatorUtils;
                    operatorVisitor.Initialize();

                    var visitor = new VisualCodeVisitor(mutationOperator.Info.Id, operatorVisitor, module.Module.Module);

                    var traverser = new VisualCodeTraverser(_filter, visitor, module);

                    traverser.Traverse(module.Module.Module);
                    visitor.PostProcess();

                    IEnumerable<MutationTarget> mutations = LimitMutationTargets(visitor.MutationTargets);


                    mergedTargets.Add(mutationOperator, new HashSet<MutationTarget>(mutations));
                    _sharedTargets.Add(mutationOperator, new HashSet<MutationTarget>(visitor.SharedTargets));

                }
                catch (Exception e)
                {
                    _svc.Logging.ShowError("Finding targets operation failed in operator: {0}. Exception: {1}".Formatted(mutationOperator.Info.Name, e.ToString()));
                    //throw new MutationException("Finding targets operation failed in operator: {0}.".Formatted(mutationOperator.Info.Name), e);
                }
            }
            return mergedTargets;
        }

        public async Task<MutationResult> ExecuteMutation(Mutant mutant, CciModuleSource moduleSource)
        {
            var type = new TypeIdentifier((INamedTypeDefinition) mutant.MutationTarget.ProcessingContext.Type.Object);
            var method = new MethodIdentifier((IMethodDefinition) mutant.MutationTarget.ProcessingContext.Method.Object);
            var filter = new MutationFilter(type.InList(), method.InList());

            _log.Debug("ExecuteMutation of: " + type+" - " +method );
            IMutationOperator mutationOperator = mutant.MutationTarget.OperatorId == null ? new IdentityOperator() :
                _mutOperators.Single(m => mutant.MutationTarget.OperatorId == m.Info.Id);
            var cci = moduleSource;
            try
            {
                _log.Info("Execute mutation of " + mutant.MutationTarget + " contained in " + mutant.MutationTarget.MethodRaw + " modules. ");
                var mutatedModules = new List<IModuleInfo>();
                var module = moduleSource.Modules.Single();



                var visitorBack = new VisualCodeVisitorBack(mutant.MutationTarget.InList(),
                    _sharedTargets.GetValues(mutationOperator, returnEmptySet: true),
                    module.Module, mutationOperator.Info.Id);

                var traverser2 = new VisualCodeTraverser(_filter, visitorBack, moduleSource);

                traverser2.Traverse(module.Module);

                visitorBack.PostProcess();

                var operatorCodeRewriter = mutationOperator.CreateRewriter();

                var rewriter = new VisualCodeRewriter(cci.Host, visitorBack.TargetAstObjects,
                    visitorBack.SharedAstObjects, _filter, operatorCodeRewriter, traverser2);

                operatorCodeRewriter.MutationTarget =
                    new UserMutationTarget(mutant.MutationTarget.Variant.Signature, mutant.MutationTarget.Variant.AstObjects);

                operatorCodeRewriter.NameTable = cci.Host.NameTable;
                operatorCodeRewriter.Host = cci.Host;
                operatorCodeRewriter.Module = module.Module;
                operatorCodeRewriter.OperatorUtils = _operatorUtils;
                operatorCodeRewriter.Initialize();

                var rewrittenModule = rewriter.Rewrite(module.Module);

                rewriter.CheckForUnfoundObjects();

                mutant.MutationTarget.Variant.AstObjects = null; //TODO: avoiding leaking memory. refactor
                mutatedModules.Add(new ModuleInfo(rewrittenModule, ""));

                List<ICciModuleSource> cci1 = new List<ICciModuleSource>();
                cci1.Add(cci);

                List<IMethodDefinition> methodMutated = new List<IMethodDefinition>();
                methodMutated.Add(mutant.MutationTarget.MethodMutated);

                var result = new MutationResult(mutant, cci/*1*/, null, mutant.MutationTarget.MethodMutated);
                mutant.MutationTarget.MethodMutated = null; //TODO: avoiding leaking memory. refactor

                return result;
            }
            catch (Exception e)
            {
                throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(mutationOperator.Info.Name), e);
            }
        }


        public async Task<MutationResult> ExecuteMutation(Mutant mutant, CciModuleSource moduleSource, CciModuleSource moduleSource2)
        {
            /*var type = new TypeIdentifier((INamedTypeDefinition) mutant.MutationTarget.ProcessingContext.Type.Object);
            var type2 = new TypeIdentifier((INamedTypeDefinition)mutant.MutationTarget2.ProcessingContext.Type.Object);//
            var method = new MethodIdentifier((IMethodDefinition) mutant.MutationTarget.ProcessingContext.Method.Object);
            var method2 = new MethodIdentifier((IMethodDefinition)mutant._mutationTarget2.ProcessingContext.Method.Object); //
            var filter = new MutationFilter(type.InList(), method.InList());
            var filter2 = new MutationFilter(type2.InList(), method2.InList());//
            */
            //_log.Debug("ExecuteMutation of: " + type+" - " +method );
            //_log.Debug("ExecuteMutation of: " + type + " - " + method + " + " + method2);
            IMutationOperator mutationOperator = mutant.MutationTarget.OperatorId == null ? new IdentityOperator() :
                _mutOperators.Single(m => mutant.MutationTarget.OperatorId == m.Info.Id);
            IMutationOperator mutationOperator2 = mutant._mutationTargets[0].OperatorId == null ? new IdentityOperator() :
                _mutOperators.Single(m => mutant._mutationTargets[0].OperatorId == m.Info.Id);
            var cci = moduleSource;
            try
            {
                //_log.Info("Execute mutation of " + mutant.MutationTarget + " contained in " + mutant.MutationTarget.MethodRaw + " modules. ");
                _log.Info("Execute mutation of " + mutant.MutationTarget + " contained in " + mutant.MutationTarget.MethodRaw + " modules and " + mutant._mutationTargets[0] + " contained in " + mutant._mutationTargets[0].MethodRaw + " modules.");
                var mutatedModules = new List<IModuleInfo>();
                var module = moduleSource.Modules.Single();

                List<MutationTarget> mutationTargets = new List<MutationTarget>();
                mutationTargets.Add(mutant.MutationTarget);
                mutationTargets.Add(mutant._mutationTargets[0]);
                List<MutationTarget> sharedtargets = new  List<MutationTarget>();
                foreach (var element in _sharedTargets.GetValues(mutationOperator, returnEmptySet: true))
                {
                     sharedtargets.Add(element);
                }
                foreach (var element in _sharedTargets.GetValues(mutationOperator2, returnEmptySet: true))
                {
                    sharedtargets.Add(element);
                }
                var visitorBack = new VisualCodeVisitorBack(mutationTargets,
                    sharedtargets,
                    module.Module, mutationOperator.Info.Id+mutationOperator2.Info.Id);


                var traverser2 = new VisualCodeTraverser(_filter, visitorBack, moduleSource);

                traverser2.Traverse(module.Module);

                visitorBack.PostProcess();

                var operatorCodeRewriter = mutationOperator.CreateRewriter();

                var rewriter = new VisualCodeRewriter(cci.Host, visitorBack.TargetAstObjects,
                    visitorBack.SharedAstObjects, _filter, operatorCodeRewriter, traverser2);

                operatorCodeRewriter.MutationTarget =
                    new UserMutationTarget(mutant.MutationTarget.Variant.Signature, mutant.MutationTarget.Variant.AstObjects);

                operatorCodeRewriter.NameTable = cci.Host.NameTable;
                operatorCodeRewriter.Host = cci.Host;
                operatorCodeRewriter.Module = module.Module;
                operatorCodeRewriter.OperatorUtils = _operatorUtils;
                operatorCodeRewriter.Initialize();

                /*var operatorCodeRewriter2 = mutationOperator2.CreateRewriter();//
                var rewriter2 = new VisualCodeRewriter(cci.Host, visitorBack.TargetAstObjects,
                    visitorBack.SharedAstObjects, _filter, operatorCodeRewriter2, traverser2);//

                operatorCodeRewriter2.MutationTarget =
                    new UserMutationTarget(mutant._mutationTarget2.Variant.Signature, mutant.MutationTarget2.Variant.AstObjects);//

                operatorCodeRewriter2.NameTable = cci.Host.NameTable;
                operatorCodeRewriter2.Host = cci.Host;
                operatorCodeRewriter2.Module = module.Module;
                operatorCodeRewriter2.OperatorUtils = _operatorUtils;
                operatorCodeRewriter2.Initialize();
                */
                var rewrittenModule = rewriter.Rewrite(module.Module);
                //var rewrittenModule2 = rewriter2.Rewrite(module.Module);//
                rewriter.CheckForUnfoundObjects();       
                //rewriter2.CheckForUnfoundObjects();//

                //2nd mutation
                
                /*IMutationOperator mutationOperator2 = mutant._mutationTarget2.OperatorId == null ? new IdentityOperator() :
                    _mutOperators.Single(m => mutant._mutationTarget2.OperatorId == m.Info.Id); //
                var cci2 = moduleSource2;
                var module2 = moduleSource2.Modules.Single();//
                var visitorBack2 = new VisualCodeVisitorBack(mutant.MutationTarget2.InList(),
                _sharedTargets.GetValues(mutationOperator2, returnEmptySet: true),
                    module2.Module, mutationOperator2.Info.Id); //

                var traverser22 = new VisualCodeTraverser(_filter, visitorBack2, moduleSource2);//
                traverser22.Traverse(module2.Module);//
                visitorBack2.PostProcess();//
                var operatorCodeRewriter2 = mutationOperator2.CreateRewriter();//
                var rewriter2 = new VisualCodeRewriter(cci2.Host, visitorBack2.TargetAstObjects,
                    visitorBack2.SharedAstObjects, _filter, operatorCodeRewriter2, traverser22);//

                operatorCodeRewriter2.MutationTarget =
                    new UserMutationTarget(mutant._mutationTarget2.Variant.Signature, mutant.MutationTarget2.Variant.AstObjects);//

                operatorCodeRewriter2.NameTable = cci2.Host.NameTable;
                operatorCodeRewriter2.Host = cci2.Host;
                operatorCodeRewriter2.Module = module2.Module;
                operatorCodeRewriter2.OperatorUtils = _operatorUtils;
                operatorCodeRewriter2.Initialize();

                var rewrittenModule2 = rewriter2.Rewrite(module2.Module);//
                rewriter2.CheckForUnfoundObjects();//
                */
                mutant.MutationTarget.Variant.AstObjects = null; //TODO: avoiding leaking memory. refactor
                mutant._mutationTargets[0].Variant.AstObjects = null; //TODO: avoiding leaking memory. refactor
                mutatedModules.Add(new ModuleInfo(rewrittenModule, ""));
                //mutatedModules.Add(new ModuleInfo(rewrittenModule2, ""));//

                //List<ICciModuleSource> cci = new List<ICciModuleSource>();
                //cci.Add(cci1);
                //cci.Add(cci2);
                List<IMethodDefinition> methodsMutated = new List<IMethodDefinition>();
                methodsMutated.Add(mutant._mutationTargets[0].MethodMutated);

                var result = new MutationResult(mutant, cci, null, mutant.MutationTarget.MethodMutated, methodsMutated);
                mutant.MutationTarget.MethodMutated = null; //TODO: avoiding leaking memory. refactor
                mutant._mutationTargets[0].MethodMutated = null; //TODO: avoiding leaking memory. refactor
                return result;
            }
            catch (Exception e)
            {
                throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(mutationOperator.Info.Name), e);
            }
             

        }


        public async Task<MutationResult> ExecuteMutation(Mutant mutant, List<CciModuleSource> moduleSource)
        {

            //Ta druga funkcja
            _log.Debug("ExecuteMutation in object: " + ToString() + GetHashCode());
            IMutationOperator mutationOperator = mutant.MutationTarget.OperatorId == null ? new IdentityOperator() :
                _mutOperators.Single(m => mutant.MutationTarget.OperatorId == m.Info.Id);
         
            try
            {
                _log.Info("Execute mutation of " + mutant.MutationTarget + " contained in " + mutant.MutationTarget.MethodRaw + " modules. ");
                var mutatedModules = new List<IModuleInfo>();

                foreach (var cci in moduleSource)
                {
                    var module = cci.Module;
                    var visitorBack = new VisualCodeVisitorBack(mutant.MutationTarget.InList(),
                        _sharedTargets.GetValues(mutationOperator, returnEmptySet: true),
                        module.Module, mutationOperator.Info.Id);
                    var traverser2 = new VisualCodeTraverser(_filter, visitorBack, cci);
                    traverser2.Traverse(module.Module);
                    visitorBack.PostProcess();
                    var operatorCodeRewriter = mutationOperator.CreateRewriter();

                    var rewriter = new VisualCodeRewriter(cci.Host, visitorBack.TargetAstObjects,
                        visitorBack.SharedAstObjects, _filter, operatorCodeRewriter, traverser2);

                    operatorCodeRewriter.MutationTarget =
                        new UserMutationTarget(mutant.MutationTarget.Variant.Signature, mutant.MutationTarget.Variant.AstObjects);


                    operatorCodeRewriter.NameTable = cci.Host.NameTable;
                    operatorCodeRewriter.Host = cci.Host;
                    operatorCodeRewriter.Module = module.Module;
                    operatorCodeRewriter.OperatorUtils = _operatorUtils;
                    operatorCodeRewriter.Initialize();

                    var rewrittenModule = rewriter.Rewrite(module.Module);
                    rewriter.CheckForUnfoundObjects();

                }

               

                mutant.MutationTarget.Variant.AstObjects = null; //TODO: avoiding leaking memory. refactor
               // mutatedModules.Add(new ModuleInfo(rewrittenModule, ""));
                List<IMethodDefinition> methodMutated = new List<IMethodDefinition>();
                methodMutated.Add(mutant.MutationTarget.MethodMutated);
                var result = new MutationResult(mutant, null, moduleSource, mutant.MutationTarget.MethodMutated);
                mutant.MutationTarget.MethodMutated = null; //TODO: avoiding leaking memory. refactor
                return result;
            }
            catch (Exception e)
            {
                throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(mutationOperator.Info.Name), e);
            }
        }




    }
}