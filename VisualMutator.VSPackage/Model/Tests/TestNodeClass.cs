namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System.Collections.ObjectModel;

    public class TestNodeClass : TestTreeNode
    {




        public TestNodeClass()
        {
            TestMethods = new ObservableCollection<TestNodeMethod>();
        }


        private ObservableCollection<TestNodeMethod> _testMethods;

        public ObservableCollection<TestNodeMethod> TestMethods
        {
            set
            {
                if (_testMethods != value)
                {
                    _testMethods = value;
                    this.RaisePropertyChanged(() => TestMethods);
                }
            }
            get
            {
                return _testMethods;
            }
        }

    }
}