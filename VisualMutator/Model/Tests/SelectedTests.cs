namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;

    public class SelectedTests
    {
        public SelectedTests(ICollection<TestId> selected, string testsDescription)
        {
            TestsDescription = testsDescription;
            TestIds = selected;
        }

        public ICollection<TestId> TestIds { get; private set; }
        public string TestsDescription { get; private set; }
    }
}