namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System.Collections.ObjectModel;

    #endregion

    public class TestNodeNamespace : TestTreeNode
    {
        private ObservableCollection<TestNodeClass> _testClasses;

        public TestNodeNamespace()
        {
            TestClasses = new ObservableCollection<TestNodeClass>();
        }

        public ObservableCollection<TestNodeClass> TestClasses
        {
            set
            {
                if (_testClasses != value)
                {
                    _testClasses = value;
                    RaisePropertyChanged(() => TestClasses);
                }
            }
            get
            {
                return _testClasses;
            }
        }
    }
}