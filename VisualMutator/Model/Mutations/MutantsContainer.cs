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

    using TypeAttributes = Mono.Cecil.TypeAttributes;

    #endregion

    public interface IMutantsContainer
    {
        MutationTestingSession PrepareSession(MutationSessionChoices choices);

        void GenerateMutantsForOperators(MutationTestingSession session, ProgressCounter progress );

        Mutant CreateChangelessMutant(MutationTestingSession session);

        void SaveMutantsToDisk(MutationTestingSession currentSession);
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
            StoredAssemblies sourceAssemblies = _assembliesManager.Store(choices.Assemblies.Select(a=>a.AssemblyDefinition).ToList());
            IList<AssemblyDefinition> reloadedAssemblies = _assembliesManager.Load(sourceAssemblies);

            var copiedTypes = ProjectTypesToCopiedAssemblies(choices.SelectedTypes, reloadedAssemblies);

            return new MutationTestingSession
            {
                OriginalAssemblies = reloadedAssemblies,
                StoredSourceAssemblies = sourceAssemblies,
                SelectedTypes = copiedTypes,
                Choices = choices,

            };
        }

        public Mutant CreateChangelessMutant(MutationTestingSession session)
        {
    
            var op = new PreOperator();
            var targets = FindTargets(op, new TypeDefinition("ns","name",new TypeAttributes()).InArrayIf(true));
            CreateMutantsForOperator(targets, session.StoredSourceAssemblies, () => 0, ProgressCounter.Inactive());
            return targets.ExecutedOperator.Mutants.First();
        }

        public void SaveMutantsToDisk(MutationTestingSession currentSession)
        {
            
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

        public void GenerateMutantsForOperators(MutationTestingSession session, ProgressCounter percentCompleted )
        {
           session.MutantsGroupedByOperators = new List<ExecutedOperator>();
            MutationRootNode root = new MutationRootNode();

            int[] id = { 1 };
            Func<int> genId = () => id[0]++;


            percentCompleted.Initialize(session.Choices.SelectedOperators.Count);

            var sw = new Stopwatch();
            //sw.Start();

            List<OperatorWithTargets> operatorsWithTargets = session.Choices.SelectedOperators
                .Select(oper =>
                {
                    percentCompleted.Progress();
                    sw.Restart();
                    var targets = FindTargets(oper, session.SelectedTypes);
                    targets.ExecutedOperator.FindTargetsTimeMiliseconds =  sw.ElapsedMilliseconds;
                    return targets;

                }).ToList();


            int times = session.Choices.MutantsCreationOptions.CreateMoreMutants ? 20 : 1;
            int allMutantsCount = operatorsWithTargets.Sum(op => op.MutationTargets.Count) * times;
            percentCompleted.Initialize(allMutantsCount);

            foreach (var op in operatorsWithTargets)
            {

                ExecutedOperator executedOperator = op.ExecutedOperator;
                sw.Restart();
                for (int i = 0; i < times; i++)
                {
                    CreateMutantsForOperator(op, session.StoredSourceAssemblies, genId, percentCompleted);
                }
                sw.Stop();
                executedOperator.MutationTimeMiliseconds = sw.ElapsedMilliseconds;

                executedOperator.UpdateDisplayedText();

                session.MutantsGroupedByOperators.Add(executedOperator);
                executedOperator.Parent = root;
                root.Children.Add(executedOperator);
                
        
            }
            root.State = MutantResultState.Untested;

            _assembliesManager.SessionEnded();


        
        }

        public class OperatorWithTargets
        {
            public List<MutationTarget> MutationTargets
            {
                get;
                set;
            }

            public IMutationOperator Operator { get; set; }

            public ExecutedOperator ExecutedOperator { get; set; }
        }


        public OperatorWithTargets FindTargets(IMutationOperator mutOperator, IList<TypeDefinition> types)
        {
            var result = new ExecutedOperator(mutOperator.Identificator,mutOperator.Name);

            List<MutationTarget> targets;
            try
            {
                targets = types.Count != 0 ? mutOperator.FindTargets(types).ToList() : new List<MutationTarget>();
            }
            catch (Exception e)
            {
                throw new MutationException("FindTargets failed on operator: {0}.".Formatted(mutOperator.Name), e);
            }
            return new OperatorWithTargets
            {
                MutationTargets = targets,
                Operator = mutOperator,
                ExecutedOperator = result,
            };

        }

        private void CreateMutantsForOperator(OperatorWithTargets oper, StoredAssemblies sourceAssemblies, 
            Func<int> generateId, ProgressCounter percentCompleted)
        {



            var results = new List<MutationContext>();
            try
            {

                
                foreach (MutationTarget mutationTarget in oper.MutationTargets)
                {
                    percentCompleted.Progress();
                    var assembliesToMutate = _assembliesManager.Load(sourceAssemblies);
                    var context = new MutationContext(mutationTarget, assembliesToMutate);
                    oper.Operator.Mutate(context);
                    results.Add(context);

                }
                
               
            }
            catch (Exception e)
            {
                throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(oper.Operator.Name), e);
            }

            foreach (MutationContext mutationResult in results)
            {
                var serializedMutant = _assembliesManager.Store(mutationResult.AssembliesToMutate.ToList());
                var mutant = new Mutant(generateId(), oper.ExecutedOperator, mutationResult.MutationTarget, serializedMutant);
                oper.ExecutedOperator.Children.Add(mutant);
            }

       
        }
    }
}