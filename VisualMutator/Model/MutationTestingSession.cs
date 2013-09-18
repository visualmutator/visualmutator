namespace VisualMutator.Model
{
    #region

    using System.Collections.Generic;
    using Extensibility;
    using Mutations.MutantsTree;
    using Mutations.Types;
    using StoringMutants;

    #endregion

    public class MutationTestingSession
    {
        public IList<ExecutedOperator> MutantsGroupedByOperators { get; set; }
        public double MutationScore { get; set; }
        public TestEnvironmentInfo TestEnvironment { get; set; }
        public ICollection<TypeIdentifier> SelectedTypes { get; set; }
        public IList<AssemblyNode> OriginalAssemblies { get; set; }
        public MutationSessionChoices Choices { get; set; }
    }
}