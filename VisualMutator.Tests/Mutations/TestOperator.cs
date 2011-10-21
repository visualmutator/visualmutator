namespace VisualMutator.Tests.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Extensibility;

    public class TestOperator : IMutationOperator
    {
        public string Name
        {
            get
            {
                return "TestOperatorName";
            }
        }

        public string Description
        {
            get
            {
                return "TestOperatorDescription";
            }
        }

        public IEnumerable<MutationTarget> FindTargets(IEnumerable<TypeDefinition> types)
        {
            yield return new MutationTarget(types.Single(t => t.Name == "Type1").Methods.Single());
            yield return new MutationTarget(types.Single(t => t.Name == "Type3").Methods.Single());

        }

        public MutationResultsCollection CreateMutants(IEnumerable<MutationTarget> targets, AssembliesToMutateFactory assembliesFactory)
        {
            int i = 0;
            var results = new MutationResultsCollection();
            foreach (var mutationTarget in targets)
            {
                var assemblyDefinitions = assembliesFactory.GetNewCopy();
                mutationTarget.GetMethod(assemblyDefinitions).Name = "MutatedMethodName" + i++;

                results.MutationResults.Add(new MutationResult
                {

                    MutatedAssemblies = assemblyDefinitions,
                    MutationTarget = mutationTarget
                });
            }

            return results;

        }


    }
}