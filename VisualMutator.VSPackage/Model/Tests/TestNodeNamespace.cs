namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

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
                    this.RaisePropertyChangedExt(() => TestClasses);
                }
            }
            get
            {
                return _testClasses;
            }
        }

    }
}