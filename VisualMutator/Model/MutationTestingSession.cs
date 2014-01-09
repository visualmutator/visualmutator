namespace VisualMutator.Model
{
    #region

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Extensibility;
    using Mutations;
    using Mutations.MutantsTree;
    using Mutations.Types;
    using StoringMutants;

    #endregion

    public class MutationTestingSession
    {
        public MutationTestingSession(TestEnvironmentInfo testEnvironmentInfo)
        {
            MutantsGrouped = new List<AssemblyNode>();
            TestEnvironment = testEnvironmentInfo;
            Filter = MutationFilter.AllowAll();
            OriginalAssemblies = new List<AssemblyNode>();
            Choices = new MutationSessionChoices();
        }

        public MutationTestingSession()
        {
        }

        public IList<AssemblyNode> MutantsGrouped { get; set; }
        public double MutationScore { get; set; }
        public TestEnvironmentInfo TestEnvironment { get; set; }
        public MutationFilter Filter { get; set; }
        public IList<AssemblyNode> OriginalAssemblies { get; set; }
        public MutationSessionChoices Choices { get; set; }
    }
}