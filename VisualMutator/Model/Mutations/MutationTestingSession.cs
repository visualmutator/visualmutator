namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;

    using Mono.Cecil;

    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests;

    public class MutationTestingSession
    {
        public MutationTestingSession()
        {
            
        }

        public IList<ExecutedOperator> MutantsGroupedByOperators { get; set; }

        public double MutantsRatio { get; set; }

        public IList<AssemblyDefinition> OriginalAssemblies { get; set; }

        public TestEnvironmentInfo TestEnvironment { get; set; }
    }
}