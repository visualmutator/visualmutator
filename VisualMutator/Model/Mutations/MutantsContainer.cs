namespace VisualMutator.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;

    using Mono.Cecil;

    using NUnit.Core;

    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.Exceptions;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.TestsTree;

    using log4net;

    #endregion

    public interface IMutantsContainer
    {

        MutationTestingSession GenerateMutantsForOperators(MutationSessionChoices choices);
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
            var root = new MutationRootNode();
         
            var sourceAssemblies = _assembliesManager.Store(choices.Assemblies);
            IList<AssemblyDefinition> assemblies = _assembliesManager.Load(sourceAssemblies);

            foreach (var op in choices.SelectedOperators)
            {

                IEnumerable<TypeDefinition> typeDefinitions =
                    choices.SelectedTypes.Select(t1 =>
                           assemblies.SelectMany(a => a.MainModule.Types)
                             .Single(t2 => t1.Module.Assembly.Name.Name == t2.Module.Assembly.Name.Name
                                 && t1.FullName == t2.FullName));

                List<MutationTarget> targets = null;
                try
                {
                    targets = op.FindTargets(typeDefinitions).ToList();
                }
                catch (Exception e)
                {
                    throw new MutationException("FindTargets failed on operator: {0}.".Formatted(op.Name), e);
                }
           

                int i = 0;
                var executedOperator = new ExecutedOperator(root, op.Name);


                var assembliesFactory = new AssembliesToMutateFactory(() => _assembliesManager.Load(sourceAssemblies));
                for (int j = 0; j < 3; j++)
                {
                    IList<MutationResult> results;
                    try
                    {
                        results = op.CreateMutants(targets, assembliesFactory).MutationResults;
                    }
                    catch (Exception e)
                    {
                        throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(op.Name), e);
                    }
       
                    foreach (var mutationResult in results)
                    {
                        var serializedMutant = _assembliesManager.Store(mutationResult.MutatedAssemblies.ToList());

                        var mutant = new Mutant(i++, executedOperator, mutationResult.MutationTarget, serializedMutant);

                         executedOperator.Children.Add(mutant);
                    }
                    
                }
                executedOperator.DisplayedText = "{0} - Mutants: {1}"
                    .Formatted(executedOperator.Name, executedOperator.Children.Count);

                
                executedOperators.Add(executedOperator);
                root.Children.Add(executedOperator);
               
            }
            root.State = MutantResultState.Waiting;

            _assembliesManager.SessionEnded();

            return new MutationTestingSession
            {
                MutantsGroupedByOperators = executedOperators,
                OriginalAssemblies = choices.Assemblies,
            };
        }

  






    }
}