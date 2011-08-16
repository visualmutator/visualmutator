namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System.Collections.ObjectModel;

    #endregion

    public class TestNodeClass : TestTreeNode
    {
        private ObservableCollection<TestNodeMethod> _testMethods;

        public TestNodeClass()
        {
            TestMethods = new ObservableCollection<TestNodeMethod>();
        }

        public ObservableCollection<TestNodeMethod> TestMethods
        {
            set
            {
                if (_testMethods != value)
                {
                    _testMethods = value;
                    RaisePropertyChanged(() => TestMethods);
                }
            }
            get
            {
                return _testMethods;
            }
        }
    }
}