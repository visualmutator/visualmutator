namespace VisualMutator.Model.Tests.TestsTree
{
    using System.Collections.Generic;
    using System.Linq;

    #region Usings

    

    #endregion

    public class TestNodeNamespace : TestTreeNode
    {
     
        public TestNodeNamespace(TestsRootNode parent, string name)
            : base(parent, name, true)
        {
        }

        public IEnumerable<TestNodeClass> TestClasses
        {
            get
            {
                return Children.Cast<TestNodeClass>();
            }
        }
    }
}