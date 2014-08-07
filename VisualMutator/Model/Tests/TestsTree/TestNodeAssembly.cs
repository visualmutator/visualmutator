namespace VisualMutator.Model.Tests.TestsTree
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using Services;

    #region Usings

    

    #endregion

    #endregion

    public class TestNodeAssembly : TestTreeNode
    {

        public TestNodeAssembly(TestsRootNode parent, string name)
            : base(parent, name, true)
        {
        }


        public string AssemblyPath
        {
            get;
            set;
        }

        public IEnumerable<TestNodeNamespace> TestNamespaces
        {
            get
            {
                return Children.Cast<TestNodeNamespace>();
            }
        }

        public List<TestsLoadContext> TestsLoadContexts { get; set; }
    }
}