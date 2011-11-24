namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;

    using Mono.Cecil;

    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests;

    public class MutationTestingOptions
    {
        public bool IsMutantVerificationEnabled { get; set; }
    }
    public class MutationTestingSession
    {
        public MutationTestingSession()
        {
            
        }

        public IList<ExecutedOperator> MutantsGroupedByOperators { get; set; }

        public double MutationScore { get; set; }

        public TestEnvironmentInfo TestEnvironment { get; set; }

        public Queue<Mutant> MutantsToTest { get; set; }

        public List<Mutant> TestedMutants { get; set; }

        public StoredAssemblies StoredSourceAssemblies { get; set; }

        public IList<TypeDefinition> SelectedTypes { get; set; }

        public IList<IMutationOperator> SelectedOperators { get; set; }

        public IList<AssemblyDefinition> OriginalAssemblies { get; set; }

        public MutationTestingOptions Options
        {
            get;
            set;
        }

        public MutationSessionChoices MutationSessionChoices { get; set; }
    }
}