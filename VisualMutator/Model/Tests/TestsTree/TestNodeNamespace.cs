namespace VisualMutator.Model.Tests.TestsTree
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using UsefulTools.CheckboxedTree;

    #region Usings

    

    #endregion

    #endregion

    public class TestNodeNamespace : TestTreeNode
    {

        public TestNodeNamespace(TestTreeNode parent, string name)
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