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
        MutationTestingSession PrepareSession(MutationSessionChoices choices);

        void GenerateMutantsForOperators(MutationTestingSession session, Action progress = null);

        Mutant CreateChangelessMutant(MutationTestingSession session);
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

        public MutationTestingSession PrepareSession(MutationSessionChoices choices)
        {
            StoredAssemblies sourceAssemblies = _assembliesManager.Store(choices.Assemblies);
            IList<AssemblyDefinition> reloadedAssemblies = _assembliesManager.Load(sourceAssemblies);

            var copiedTypes = ProjectTypesToCopiedAssemblies(choices.SelectedTypes, reloadedAssemblies);

            return new MutationTestingSession
            {
                OriginalAssemblies = reloadedAssemblies,
                StoredSourceAssemblies = sourceAssemblies,
                SelectedTypes = copiedTypes,
                SelectedOperators = choices.SelectedOperators,
                Options = CreateDefaultOptions()
            };
        }

        private MutationTestingOptions CreateDefaultOptions()
        {
            return new MutationTestingOptions
            {
                IsMutantVerificationEnabled = true
            };
        }

        public Mutant CreateChangelessMutant(MutationTestingSession session)
        {
    
            var op = new PreOperator();

            return CreateMutantsForOperator(op, 
                session.SelectedTypes, session.StoredSourceAssemblies, ()=>0).Mutants.Single();
        }

        public IList<TypeDefinition> ProjectTypesToCopiedAssemblies(IList<TypeDefinition> sourceTypes, 
            IList<AssemblyDefinition> destinationAssemblies)
        {
            return sourceTypes
              .Select(t1 =>
                      destinationAssemblies.SelectMany(a => a.MainModule.Types)
                          .Single(t2 => t1.Module.Assembly.Name.Name == t2.Module.Assembly.Name.Name
                                        && t1.FullName == t2.FullName)).ToList();

        }

        public void GenerateMutantsForOperators(MutationTestingSession session, Action progress = null)
        {
           session.MutantsGroupedByOperators = new List<ExecutedOperator>();
            MutationRootNode root = new MutationRootNode();

            int[] i = { 0 };
            Func<int> genId = () => i[0]++;


            foreach (IMutationOperator op in session.SelectedOperators)
            {
                var sw = new Stopwatch();
                sw.Start();


                ExecutedOperator executedOperator = CreateMutantsForOperator(op,
                    session.SelectedTypes, session.StoredSourceAssemblies, genId);

                executedOperator.DisplayedText = "{0} - Mutants: {1}"
                    .Formatted(executedOperator.Name, executedOperator.Children.Count);

                session.MutantsGroupedByOperators.Add(executedOperator);
                executedOperator.Parent = root;
                root.Children.Add(executedOperator);
                sw.Stop();
                executedOperator.MutationTimeMiliseconds = sw.ElapsedMilliseconds;
                if (progress != null)
                {
                    progress();
                }
            }
            root.State = MutantResultState.Untested;

            _assembliesManager.SessionEnded();


        
        }


        private ExecutedOperator CreateMutantsForOperator(IMutationOperator mutOperator,
            IEnumerable<TypeDefinition> types, StoredAssemblies sourceAssemblies, 
            Func<int> generateId  )
        {
            var result = new ExecutedOperator( mutOperator.Name);

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