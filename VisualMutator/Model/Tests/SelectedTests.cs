namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;

    public class SelectedTests
    {
        private readonly List<string> _minimalSelectionList;

        public List<string> MinimalSelectionList
        {
            get { return _minimalSelectionList; }
        }

        public SelectedTests(ICollection<TestId> selected, List<string> minimalSelectionList)
        {
            _minimalSelectionList = minimalSelectionList;
            TestIds = selected;
        }

        public ICollection<TestId> TestIds { get; private set; }
        public string TestsDescription { get {return string.Join(" ", _minimalSelectionList).Trim();
            } }
    }
}