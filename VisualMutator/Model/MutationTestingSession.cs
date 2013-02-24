namespace VisualMutator.Model
{
    using System.Collections.Generic;
    using Mutations.MutantsTree;
    using StoringMutants;
    using VisualMutator.Extensibility;

    public class MutationTestingSession
    {
        public MutationTestingSession()
        {
            
        }

        public IList<ExecutedOperator> MutantsGroupedByOperators { get; set; }

        public double MutationScore { get; set; }

        public TestEnvironmentInfo TestEnvironment { get; set; }

    //    public Queue<Mutant> MutantsToTest { get; set; }

     //   public List<Mutant> TestedMutants { get; set; }

        public StoredAssemblies StoredSourceAssemblies { get; set; }

        public ICollection<TypeIdentifier> SelectedTypes { get; set; }

    //    public IList<IMutationOperator> SelectedOperators { get; set; }

        public AssembliesProvider OriginalAssemblies { get; set; }

      

        public MutationSessionChoices Choices { get; set; }
    }
}