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

        private readonly OptionsModel _options;//AKB

        public MutantsContainer(
            IMutationExecutor mutationExecutor,
            OriginalCodebase originalCodebase,
            OptionsModel options//AKB
            )
        {
            _mutationExecutor = mutationExecutor;
            _originalCodebase = originalCodebase;
            _options = options;//AKB

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

            assNodes = modifyAssNodes(assNodes);

            root.State = MutantResultState.Untested;

            return assNodes;
        }

        private MutationNode GroupOrMutant(IGrouping<string, MutationTarget> byGroupGrouping)
        {
            if (byGroupGrouping.Count() == 1)
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


            foreach (var node in assemblyNode.Children.Where(n => n.Children.Count == 0).ToList())
            {
                assemblyNode.Children.Remove(node);
            }
            return assemblyNode;
        }
        //AKB
        private List<AssemblyNode> modifyAssNodes(List<AssemblyNode> assNodes)
        {
            //List<AssemblyNode> newAssNodes = new List<AssemblyNode>();
            if (_options.MutationOrder == 2)
            {
                foreach (var assembly in assNodes)
                {
                    foreach (var assProjectNode in assembly.Children)
                    {
                        foreach (var assFileNode in assProjectNode.Children)
                        {
                            foreach (var assMutantsNode in assFileNode.Children)
                            {
                                int nrOfNodes = assMutantsNode.Children.Count;
                                for (int i = 0; i < nrOfNodes; i++)
                                {
                                    if (assMutantsNode.Children[i].Children != null)
                                    {
                                        for (int j = 0; j < assMutantsNode.Children[i].Children.Count; j++)
                                        {
                                            if (((Mutant)assMutantsNode.Children[i].Children[j])._mutationTargets.Count == 0 && ((Mutant)assMutantsNode.Children[i].Children[j])._nrTimesWasAdded == 0)
                                            {
                                                Mutant partner = findPartner(assNodes, ((Mutant)assMutantsNode.Children[i].Children[j]));
                                                if (partner.Id == "First Order Mutant")
                                                {
                                                    ((Mutant)assMutantsNode.Children[i].Children[j]).Id += " - " + partner.Id;
                                                }
                                                else
                                                {
                                                    ((Mutant)assMutantsNode.Children[i].Children[j]).Id += " + " + partner.Id;
                                                }
                                                ((Mutant)assMutantsNode.Children[i].Children[j])._mutationTargets.Add(partner.MutationTarget);
                                                ((Mutant)assMutantsNode.Children[i].Children[j]).UpdateDisplayedText();
                                            }                                        
                                        }
                                    }
                                    else
                                    {
                                        if (((Mutant)assMutantsNode.Children[i])._mutationTargets.Count == 0 && ((Mutant)assMutantsNode.Children[i])._nrTimesWasAdded == 0)
                                        {
                                            Mutant partner = findPartner(assNodes, ((Mutant)assMutantsNode.Children[i]));
                                            if (partner.Id == "First Order Mutant")
                                            {
                                                ((Mutant)assMutantsNode.Children[i]).Id += " - " + partner.Id;
                                            }
                                            else
                                            {
                                                ((Mutant)assMutantsNode.Children[i]).Id += " + " + partner.Id;
                                            }
                                            ((Mutant)assMutantsNode.Children[i])._mutationTargets.Add(partner.MutationTarget);
                                            ((Mutant)assMutantsNode.Children[i]).UpdateDisplayedText();
                                        }   
                                    }
                                }
                            }
                        }
                    }
                }
                assNodes = deleteUsedMutants(assNodes);
            }
            return assNodes;
        }

        private Mutant findPartner(List<AssemblyNode> assNodes, Mutant mutant)
        {
            bool existsAlreadyUsed = false;
            int lowestNrOfRep = 0;
            foreach (var assembly in assNodes)
            {
                foreach (var assProjectNode in assembly.Children)
                {
                    foreach (var assFileNode in assProjectNode.Children)
                    {
                        foreach (var assMutantsNode in assFileNode.Children)
                        {
                            for (int i = assMutantsNode.Children.Count - 1; i >= 0; i--)
                            {
                                if (assMutantsNode.Children[i].Children != null)
                                {
                                    for (int j = assMutantsNode.Children[i].Children.Count - 1; j >= 0; j--)
                                    {
                                        if (((Mutant)assMutantsNode.Children[i].Children[j]).Description == mutant.Description && ((Mutant)assMutantsNode.Children[i].Children[j])._mutationTargets.Count == 0 && ((Mutant)assMutantsNode.Children[i].Children[j]).Id != mutant.Id && getIdWithoutNr(((Mutant)assMutantsNode.Children[i].Children[j]).Id) == getIdWithoutNr(mutant.Id))
                                        {
                                            if (((Mutant)assMutantsNode.Children[i].Children[j])._nrTimesWasAdded == 0)
                                            {
                                                ((Mutant)assMutantsNode.Children[i].Children[j])._nrTimesWasAdded++;
                                                return ((Mutant)assMutantsNode.Children[i].Children[j]);
                                            }
                                            else
                                            {
                                                existsAlreadyUsed = true;
                                                if (lowestNrOfRep == 0)
                                                {
                                                    lowestNrOfRep = ((Mutant)assMutantsNode.Children[i].Children[j])._nrTimesWasAdded;
                                                }
                                                else
                                                {
                                                    lowestNrOfRep = Math.Min(lowestNrOfRep, ((Mutant)assMutantsNode.Children[i].Children[j])._nrTimesWasAdded);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (((Mutant)assMutantsNode.Children[i]).Description == mutant.Description && ((Mutant)assMutantsNode.Children[i])._mutationTargets.Count == 0 && ((Mutant)assMutantsNode.Children[i]).Id != mutant.Id && getIdWithoutNr(((Mutant)assMutantsNode.Children[i]).Id) == getIdWithoutNr(mutant.Id))
                                    {
                                        if (((Mutant)assMutantsNode.Children[i])._nrTimesWasAdded == 0)
                                        {
                                            ((Mutant)assMutantsNode.Children[i])._nrTimesWasAdded++;
                                            return ((Mutant)assMutantsNode.Children[i]);
                                        }
                                        else
                                        {
                                            existsAlreadyUsed = true;
                                            if (lowestNrOfRep == 0)
                                            {
                                                lowestNrOfRep = ((Mutant)assMutantsNode.Children[i])._nrTimesWasAdded;
                                            }
                                            else
                                            {
                                                lowestNrOfRep = Math.Min(lowestNrOfRep, ((Mutant)assMutantsNode.Children[i])._nrTimesWasAdded);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (existsAlreadyUsed)
            {
                foreach (var assembly in assNodes)
                {
                    foreach (var assProjectNode in assembly.Children)
                    {
                        foreach (var assFileNode in assProjectNode.Children)
                        {
                            foreach (var assMutantsNode in assFileNode.Children)
                            {
                                for (int i = assMutantsNode.Children.Count - 1; i >= 0; i--)
                                {
                                    if (assMutantsNode.Children[i].Children != null)
                                    {
                                        for (int j = assMutantsNode.Children[i].Children.Count - 1; j >= 0; j--)
                                        {
                                            if (((Mutant)assMutantsNode.Children[i].Children[j]).Description == mutant.Description && ((Mutant)assMutantsNode.Children[i].Children[j])._mutationTargets.Count == 0 && ((Mutant)assMutantsNode.Children[i].Children[j]).Id != mutant.Id && getIdWithoutNr(((Mutant)assMutantsNode.Children[i].Children[j]).Id) == getIdWithoutNr(mutant.Id) && ((Mutant)assMutantsNode.Children[i].Children[j])._nrTimesWasAdded == lowestNrOfRep)
                                            {
                                                ((Mutant)assMutantsNode.Children[i].Children[j])._nrTimesWasAdded++;
                                                return ((Mutant)assMutantsNode.Children[i].Children[j]);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (((Mutant)assMutantsNode.Children[i]).Description == mutant.Description && ((Mutant)assMutantsNode.Children[i])._mutationTargets.Count == 0 && ((Mutant)assMutantsNode.Children[i]).Id != mutant.Id && getIdWithoutNr(((Mutant)assMutantsNode.Children[i]).Id) == getIdWithoutNr(mutant.Id) && ((Mutant)assMutantsNode.Children[i])._nrTimesWasAdded == lowestNrOfRep)
                                        {
                                            ((Mutant)assMutantsNode.Children[i])._nrTimesWasAdded++;
                                            return ((Mutant)assMutantsNode.Children[i]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Mutant mutantCopy = new Mutant("First Order Mutant", mutant.MutationTarget);
                mutantCopy.Id = "First Order Mutant";
                return mutantCopy;
            }
            return mutant;
        }

        private List<AssemblyNode> deleteUsedMutants(List<AssemblyNode> assNodes)
        {
            foreach (var assembly in assNodes)
            {
                foreach (var assProjectNode in assembly.Children)
                {
                    foreach (var assFileNode in assProjectNode.Children)
                    {
                        foreach (var assMutantsNode in assFileNode.Children)
                        {
                            int nrOfNodes = assMutantsNode.Children.Count;
                            for (int i = 0; i < nrOfNodes; i++)
                            {
                                if (assMutantsNode.Children[i].Children != null)
                                {
                                    for (int j = 0; j < assMutantsNode.Children[i].Children.Count; j++)
                                    {
                                        if (((Mutant)assMutantsNode.Children[i].Children[j])._nrTimesWasAdded != 0)
                                        {
                                            assMutantsNode.Children[i].Children.RemoveAt(j);
                                            j--;
                                        }
                                    }
                                    if (assMutantsNode.Children[i].Children.Count == 0)
                                    {
                                        assMutantsNode.Children.RemoveAt(i);
                                        i--;
                                    }
                                    nrOfNodes = assMutantsNode.Children.Count;
                                }
                                else
                                {
                                    if (((Mutant)assMutantsNode.Children[i])._nrTimesWasAdded != 0)
                                    {
                                        assMutantsNode.Children.RemoveAt(i);
                                        i--;
                                    }
                                    nrOfNodes = assMutantsNode.Children.Count;
                                }
                            }
                        }
                    }
                }
            }
                return assNodes;
        }

        private string getIdWithoutNr(string id)
        {
            return id.Substring(0, id.IndexOf("#"));
        }
    }
}