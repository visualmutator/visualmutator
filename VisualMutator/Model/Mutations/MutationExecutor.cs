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
    using log4net;
    using Microsoft.Cci;
    using MutantsTree;
    using Operators;
    using StoringMutants;
    using Strilanc.Value;
    using Types;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    public interface IMutationExecutor
    {
        MultiDictionary<IMutationOperator, MutationTarget> FindTargets(IModuleInfo module);
        Task<MutationResult> ExecuteMutation(Mutant mutant, CciModuleSource moduleSource);
    }

    public class MutationExecutor : IMutationExecutor
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MutationSessionChoices _choices;
        private readonly IOperatorUtils _operatorUtils;
        private readonly MutantsCreationOptions _options;
        private readonly MutationFilter _filter;
        private readonly IList<IMutationOperator> _mutOperators;
        private MultiDictionary<IMutationOperator, MutationTarget> _sharedTargets;

        public MutationExecutor(
        MutationSessionChoices choices,
        IOperatorUtils operatorUtils
        )
        {
            _choices = choices;
            _operatorUtils = operatorUtils;
            _options = _choices.MutantsCreationOptions;
            _filter = _choices.Filter;
            _mutOperators = _choices.SelectedOperators;
            _sharedTargets = new MultiDictionary<IMutationOperator, MutationTarget>();
        }
       

        private IList<MutationTarget> LimitMutationTargets(IEnumerable<MutationTarget> targets)
        {
            // return targets.ToList();
            var mapping = targets//.Shuffle()
                                 //  .SelectMany(pair => pair.Item2.Select(t => Tuple.Create(pair.Item1, t))).Shuffle()
                .Take(_options.MaxNumerOfMutantPerOperator).ToList();
            return mapping;
        }


        public MultiDictionary<IMutationOperator, MutationTarget> FindTargets(IModuleInfo module)
        {
            _log.Info("Finding targets for module: " + module.Name);
            _log.Info("Using mutation operators: " + _mutOperators.Select(_ => _.Info.Id)
                .MayAggregate((a, b) => a + "," + b).Else("None"));

            var mergedTargets = new MultiDictionary<IMutationOperator, MutationTarget>();
            _sharedTargets = new MultiDictionary<IMutationOperator, MutationTarget>();
            foreach (var mutationOperator in _mutOperators)
            {
                try
                {
                    var ded = mutationOperator.CreateVisitor();
                    IOperatorCodeVisitor operatorVisitor = ded;
                    operatorVisitor.Host = _choices.WhiteSource.Host;
                    operatorVisitor.OperatorUtils = _operatorUtils;
                    operatorVisitor.Initialize();

                    var visitor = new VisualCodeVisitor(mutationOperator.Info.Id, operatorVisitor, module.Module);

                    var traverser = new VisualCodeTraverser(_filter, visitor);

                    traverser.Traverse(module.Module);
                    visitor.PostProcess();

                    IEnumerable<MutationTarget> mutations = LimitMutationTargets(visitor.MutationTargets);


                    mergedTargets.Add(mutationOperator, new HashSet<MutationTarget>(mutations));
                    _sharedTargets.Add(mutationOperator, new HashSet<MutationTarget>(visitor.SharedTargets));

                }
                catch (Exception e)
                {
                    throw new MutationException("Finding targets operation failed in operator: {0}.".Formatted(mutationOperator.Info.Name), e);
                }
            }
            return mergedTargets;
        }



        public async Task<MutationResult> ExecuteMutation(Mutant mutant, CciModuleSource moduleSource)
        {
            

            _log.Debug("ExecuteMutation in object: " + ToString() + GetHashCode());
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
                var traverser2 = new VisualCodeTraverser(_filter, visitorBack);
                traverser2.Traverse(module.Module);
                visitorBack.PostProcess();
                var operatorCodeRewriter = mutationOperator.CreateRewriter();

                var rewriter = new VisualCodeRewriter(cci.Host, visitorBack.TargetAstObjects,
                    visitorBack.SharedAstObjects, _filter, operatorCodeRewriter);

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

                var result = new MutationResult(new SimpleModuleSource(mutatedModules), cci,
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