namespace VisualMutator.Model.Mutations
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using Extensibility;
    using log4net;
    using Microsoft.Cci.MutableCodeModel;
    using MutantsTree;
    using Operators;
    using StoringMutants;
    using Strilanc.Value;
    using Types;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using Assembly = Microsoft.Cci.MutableCodeModel.Assembly;

    #endregion

    public interface IMutantsContainer
    {
        Mutant CreateEquivalentMutant(out AssemblyNode assemblyNode);
        MutationResult ExecuteMutation(Mutant mutant, ProgressCounter percentCompleted, CciModuleSource moduleSource);
        IList<AssemblyNode> InitMutantsForOperators(ProgressCounter percentCompleted);
    }

    public class MutantsContainer : IMutantsContainer
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MutationSessionChoices _choices;
        private readonly IWhiteCache _whiteCache;
        private readonly IOperatorUtils _operatorUtils;
        private readonly MutantsCreationOptions _options;
        private readonly MutationFilter _filter;
        private readonly ICollection<IMutationOperator> _mutOperators;
        private MultiDictionary<IMutationOperator, MutationTarget> _sharedTargets;

        public MutantsContainer(
            MutationSessionChoices choices,
            IWhiteCache whiteCache,
            IOperatorUtils operatorUtils
            )
        {
            _choices = choices;
            _whiteCache = whiteCache;
            _operatorUtils = operatorUtils;
            _options = _choices.MutantsCreationOptions;
            _filter = _choices.Filter;
            _mutOperators = _choices.SelectedOperators;
        }

 

        public Mutant CreateEquivalentMutant(out AssemblyNode assemblyNode)
        {
            _sharedTargets = new MultiDictionary<IMutationOperator, MutationTarget>();
            assemblyNode = new AssemblyNode("All modules");
            var nsNode = new TypeNamespaceNode(assemblyNode, "");
            assemblyNode.Children.Add(nsNode);
            var typeNode = new TypeNode(nsNode, "");
            nsNode.Children.Add(typeNode);
            var methodNode = new MethodNode(typeNode, "", null, true);
            typeNode.Children.Add(methodNode);
            var group = new MutantGroup("Testing original program", methodNode);
            var target = new MutationTarget(new MutationVariant())
                         {
                             Name = "Original program",
                         };
            var mutant = new Mutant("0", group, target);
           
            group.Children.Add(mutant);
            methodNode.Children.Add(group);
            group.UpdateDisplayedText();
            return mutant;
        }


        public IList<AssemblyNode> InitMutantsForOperators(ProgressCounter percentCompleted)
        {
            var mutantsGroupedByOperators = new List<ExecutedOperator>();
            var root = new MutationRootNode();

            int[] id = { 1 };
            Func<int> genId = () => id[0]++;

            var originalModules = _choices.WhiteSource;//_whiteCache.GetWhiteModules();
            percentCompleted.Initialize(originalModules.Modules.Count);
            var subProgress = percentCompleted.CreateSubprogress();

            var sw = new Stopwatch();

            var assNodes = new List<AssemblyNode>();
            foreach (var module in originalModules.Modules)
            {
                sw.Restart();

                AssemblyNode assemblyNode = FindTargets(module);
                assNodes.Add(assemblyNode);
                percentCompleted.Progress();
            }


            root.State = MutantResultState.Untested;

            return assNodes;
        }

        private IList<MutationTarget> LimitMutationTargets(IEnumerable<MutationTarget> targets)
        {
           // return targets.ToList();
            var mapping = targets//.Shuffle()
              //  .SelectMany(pair => pair.Item2.Select(t => Tuple.Create(pair.Item1, t))).Shuffle()
                .Take(_options.MaxNumerOfMutantPerOperator).ToList();
            return mapping;
        }


        public AssemblyNode FindTargets(IModuleInfo module)
        {
            _log.Info("Finding targets for module: " + module.Name);
            _log.Info("Using mutation operators: " + _mutOperators.Select(_=>_.Info.Id)
                .MayAggregate((a,b)=>a+","+b).Else("None"));

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
            var assemblyNode = BuildMutantsTree(module.Name, mergedTargets);
                
            _log.Info("Found total of: " + mergedTargets.Values.Count() + " mutation targets in "+assemblyNode.Name);
            return assemblyNode;
            
        }

        private AssemblyNode BuildMutantsTree(string moduleName,
            MultiDictionary<IMutationOperator, MutationTarget> mutationTargets)
        {

            var assemblyNode = new AssemblyNode(moduleName);

            System.Action<CheckedNode, ICollection<MutationTarget>> typeNodeCreator = (parent, targets) =>
            {
                var typeNodes =
                    from t in targets
                    orderby t.TypeName
                    group t by t.TypeName
                    into byTypeGrouping
                    select new TypeNode(parent, byTypeGrouping.Key,
                        from gr in byTypeGrouping
                        group gr by gr.MethodRaw.Name.Value
                        into byMethodGrouping
                        orderby byMethodGrouping.Key
                        let md = byMethodGrouping.First().MethodRaw
                        select new MethodNode(md.Name.Value, md,
                            from gr in byMethodGrouping
                            group gr by gr.GroupName
                            into byGroupGrouping
                            orderby byGroupGrouping.Key
                            select new MutantGroup(byGroupGrouping.Key,
                                from mutationTarget in byGroupGrouping
                                select new Mutant(mutationTarget.Id, mutationTarget)
                                )
                            )
                        );

                parent.Children.AddRange(typeNodes);


//                foreach (var byTypeGrouping in targets
//                        .OrderBy(t => t.TypeName)
//                        .GroupBy(t => t.TypeName))
//                    {
//                        var type = new TypeNode(parent, byTypeGrouping.Key);
//                        parent.Children.Add(type);
//
//                        var groupedByMethodTargets = byTypeGrouping.GroupBy(target => target.MethodRaw.Name.Value);
//                        foreach (var byMethodGrouping in groupedByMethodTargets.OrderBy(g => g.Key))
//                        {
//                            var md = byMethodGrouping.First().MethodRaw;
//                            var method = new MethodNode(type, md.Name.Value, md, true);
//                            type.Children.Add(method);
//
//                            var groupedTargets = byMethodGrouping.GroupBy(target => target.GroupName);
//                            foreach (var byGroupGrouping in groupedTargets.OrderBy(g => g.Key))
//                            {
//                                var group = new MutantGroup(byGroupGrouping.Key, method);
//                                method.Children.Add(group);
//                                foreach (var mutationTarget in byGroupGrouping)
//                                {
//                                    var mutant = new Mutant(mutationTarget.Id, group, mutationTarget);
//                                    group.Children.Add(mutant);
//
//                                }
//                                group.UpdateDisplayedText();
//                            }
//                        }
//                    }
                };

            Func<MutationTarget, string> namespaceExtractor = target => target.NamespaceName;

            NamespaceGrouper<MutationTarget, CheckedNode>.GroupTypes(assemblyNode, 
                namespaceExtractor, 
                (parent, name) => new TypeNamespaceNode(parent, name), 
                typeNodeCreator,
                mutationTargets.Values.SelectMany(a => a).ToList());


            foreach (var node in assemblyNode.Children.Where(n=>n.Children.Count == 0).ToList())
            {
                assemblyNode.Children.Remove(node);
            }
            return assemblyNode;
        }

        public MutationResult ExecuteMutation(Mutant mutant,  ProgressCounter percentCompleted, 
            CciModuleSource moduleSource)
        {
            _log.Debug("ExecuteMutation in object: " +ToString()+ GetHashCode());
            IMutationOperator mutationOperator = mutant.MutationTarget.OperatorId == null? new IdentityOperator() : 
                _mutOperators.Single(m => mutant.MutationTarget.OperatorId == m.Info.Id);
            var cci = moduleSource;
            try
            {
                _log.Info("Execute mutation of " + mutant.MutationTarget + " contained in " + mutant.MutationTarget.MethodRaw + " modules. " );
                var mutatedModules = new List<IModuleInfo>();
                foreach (var module in moduleSource.Modules)
                {
                    percentCompleted.Progress();
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

                    var rewrittenModule = (Assembly) rewriter.Rewrite(module.Module);

                    rewriter.CheckForUnfoundObjects();

                    mutant.MutationTarget.Variant.AstObjects = null; //TODO: saving memory. refactor
                    mutatedModules.Add(new ModuleInfo(rewrittenModule, ""));
                    
                }
                var result = new MutationResult(new SimpleModuleSource(mutatedModules), cci, 
                    mutant.MutationTarget.MethodMutated);
                mutant.MutationTarget.MethodMutated = null; //TODO: saving memory. refactor
                return result;
            }
            catch (Exception e)
            {
                throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(mutationOperator.Info.Name), e);
            }
        }
    }
}