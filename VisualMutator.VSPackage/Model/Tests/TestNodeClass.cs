namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

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
                    this.RaisePropertyChangedExt(() => TestMethods);
                }
            }
            get
            {
                return _testMethods;
            }
        }

    }
}