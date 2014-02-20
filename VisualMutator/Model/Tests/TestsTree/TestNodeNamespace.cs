namespace VisualMutator.Model.Tests.TestsTree
{
    #region

    using System.Collections.Generic;
    using System.Linq;

    #region Usings

    

    #endregion

    #endregion

    public class TestNodeNamespace : TestTreeNode
    {
     
        public TestNodeNamespace(TestNodeAssembly parent, string name)
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