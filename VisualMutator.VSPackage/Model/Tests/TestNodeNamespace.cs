namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System.Collections.ObjectModel;

    public class TestNodeNamespace : TestTreeNode
    {




        public TestNodeNamespace()
        {
            TestClasses = new ObservableCollection<TestNodeClass>();
        }


        private ObservableCollection<TestNodeClass> _testClasses;

        public ObservableCollection<TestNodeClass> TestClasses
        {
            set
            {
                if (_testClasses != value)
                {
                    _testClasses = value;
                    this.RaisePropertyChanged(() => TestClasses);
                }
            }
            get
            {
                return _testClasses;
            }
        }

    }
}