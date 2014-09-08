namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
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
    using Traversal;
    using Types;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    public interface IMutationExecutor
    {
        MultiDictionary<IMutationOperator, MutationTarget> FindTargets(CciModuleSource module, ProgressCounter subProgress);
        Task<MutationResult> ExecuteMutation(Mutant mutant, CciModuleSource moduleSource);
        Task<MutationResult> ExecuteMutation(Mutant mutant, List<CciModuleSource> moduleSource);
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

                var result = new MutationResult(mutant, cci, null,
                    mutant.MutationTarget.MethodMutated);
                mutant.MutationTarget.MethodMutated = null; //TODO: avoiding leaking memory. refactor
                return result;
            }
            catch (Exception e)
            {
                throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(mutationOperator.Info.Name), e);
            }
        }


        public async Task<MutationResult> ExecuteMutation(Mutant mutant, List<CciModuleSource> moduleSource)
        {


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

                var result = new MutationResult(mutant, null, moduleSource,
                    mutant.MutationTarget.MethodMutated);
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