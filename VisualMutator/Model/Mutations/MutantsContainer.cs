namespace VisualMutator.Model.Mutations
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
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
        IList<AssemblyNode> InitMutantsForOperators(ProgressCounter percentCompleted);
    }

    public class MutantsContainer : IMutantsContainer
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IMutationExecutor _mutationExecutor;
        private readonly OriginalCodebase _originalCodebase;

        public MutantsContainer(
            IMutationExecutor mutationExecutor,
            OriginalCodebase originalCodebase
            )
        {
            _mutationExecutor = mutationExecutor;
            _originalCodebase = originalCodebase;
        }

 

        public Mutant CreateEquivalentMutant(out AssemblyNode assemblyNode)
        {
            
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
            var root = new MutationRootNode();

            int[] id = { 1 };
            Func<int> genId = () => id[0]++;

            //var originalModules = _choices.WhiteSource;//_whiteCache.GetWhiteModules();
            percentCompleted.Initialize(_originalCodebase.ModulesToMutate.Count);
            var subProgress = percentCompleted.CreateSubprogress();

            var sw = new Stopwatch();

            var assNodes = new List<AssemblyNode>();
            foreach (var module in _originalCodebase.ModulesToMutate)
            {
                sw.Restart();

                var mergedTargets = _mutationExecutor.FindTargets(module, subProgress);
                var assemblyNode = BuildMutantsTree(module.Module.Name, mergedTargets);

                _log.Info("Found total of: " + mergedTargets.Values.Count() + " mutation targets in " + assemblyNode.Name);

                assNodes.Add(assemblyNode);
                percentCompleted.Progress();
            }
            root.State = MutantResultState.Untested;

            return assNodes;
        }

        private MutationNode GroupOrMutant(IGrouping<string, MutationTarget> byGroupGrouping)
        {
            if(byGroupGrouping.Count() == 1)
            {
                var mutationTarget = byGroupGrouping.First();
                return new Mutant(mutationTarget.Id, mutationTarget);
            }
            else
            {
                return new MutantGroup(byGroupGrouping.Key,
                    from mutationTarget in byGroupGrouping
                    select new Mutant(mutationTarget.Id, mutationTarget)
                    );
            }
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
                            select GroupOrMutant(byGroupGrouping)
                        )
                    );

                parent.Children.AddRange(typeNodes);
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

    }
}