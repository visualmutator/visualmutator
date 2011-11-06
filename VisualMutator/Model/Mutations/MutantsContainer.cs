namespace VisualMutator.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure;

    using Mono.Cecil;
    using Mono.Collections.Generic;

    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.Exceptions;
    using VisualMutator.Model.Mutations.Structure;

    using log4net;

    #endregion

    public interface IMutantsContainer
    {
        MutationTestingSession GenerateMutantsForOperators(MutationSessionChoices choices);
    }

    class PreOperator : IMutationOperator
    {
        public string Name
        {
            get
            {
                return "PreOperator";
            }
        }

        public string Description
        {
            get
            {
                return "PreOperator";
            }
        }

        public IEnumerable<MutationTarget> FindTargets(IEnumerable<TypeDefinition> types)
        {
            yield return new MutationTarget();
        }

        public void Mutate(MutationTarget target, IList<AssemblyDefinition> assembliesToMutate)
        {
            
        }

    }


    public class MutantsContainer : IMutantsContainer
    {
        private readonly IAssembliesManager _assembliesManager;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MutantsContainer(IAssembliesManager assembliesManager
            )
        {
            _assembliesManager = assembliesManager;
        }

        public MutationTestingSession GenerateMutantsForOperators(MutationSessionChoices choices)
        {
            var executedOperators = new List<ExecutedOperator>();
            MutationRootNode root = new MutationRootNode();

            StoredAssemblies sourceAssemblies = _assembliesManager.Store(choices.Assemblies);
            IList<AssemblyDefinition> assemblies = _assembliesManager.Load(sourceAssemblies);

            int[] i = { 0 };
            Func<int> genId = () => i[0]++;

           
            foreach (IMutationOperator op in choices.SelectedOperators)
            {
                var sw = new Stopwatch();
                sw.Start();

                IList<TypeDefinition> typeDefinitions = choices.SelectedTypes
                    .Select(t1 =>
                            assemblies.SelectMany(a => a.MainModule.Types)
                                .Single(t2 => t1.Module.Assembly.Name.Name == t2.Module.Assembly.Name.Name
                                              && t1.FullName == t2.FullName)).ToList();

               
                ExecutedOperator executedOperator = CreateMutantsForOperator(op, root,
                    typeDefinitions, sourceAssemblies, genId);

                executedOperator.DisplayedText = "{0} - Mutants: {1}"
                    .Formatted(executedOperator.Name, executedOperator.Children.Count);

                executedOperators.Add(executedOperator);
                root.Children.Add(executedOperator);
                sw.Stop();
                executedOperator.MutationTimeMiliseconds = sw.ElapsedMilliseconds;
            }
            root.State = MutantResultState.Untested;

            _assembliesManager.SessionEnded();

            return new MutationTestingSession
            {
                MutantsGroupedByOperators = executedOperators,
                OriginalAssemblies = choices.Assemblies,
            };
        }


        private ExecutedOperator CreateMutantsForOperator(IMutationOperator mutOperator,
                                                          MutationRootNode root, 
            IEnumerable<TypeDefinition> types, StoredAssemblies sourceAssemblies, Func<int> generateId )
        {
            var result = new ExecutedOperator(root, mutOperator.Name);

            List<MutationTarget> targets;
            try
            {
                targets = mutOperator.FindTargets(types).ToList();
            }
            catch (Exception e)
            {
                throw new MutationException("FindTargets failed on operator: {0}.".Formatted(mutOperator.Name), e);
            }


            IList<MutationResult> results = new List<MutationResult>();
            try
            {
                foreach (MutationTarget mutationTarget in targets)
                {
                    var assembliesToMutate = _assembliesManager.Load(sourceAssemblies);
                    mutOperator.Mutate(mutationTarget, assembliesToMutate);
                    results.Add(new MutationResult(mutationTarget, assembliesToMutate));
                }
            }
            catch (Exception e)
            {
                throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(mutOperator.Name), e);
            }

            foreach (MutationResult mutationResult in results)
            {
                var serializedMutant = _assembliesManager.Store(mutationResult.MutatedAssemblies.ToList());
                var mutant = new Mutant(generateId(), result, mutationResult.MutationTarget, serializedMutant);
                result.Children.Add(mutant);
            }
 
            return result;
        }
    }
}