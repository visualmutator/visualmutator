namespace VisualMutator.Model.Tests.TestsTree
{
    using System.Collections.Generic;
    using System.Linq;

    public class TestsRootNode : TestTreeNode
    {
        public TestsRootNode()
            : base(null, "", true)
        {

        }

        public IEnumerable<TestNodeAssembly> TestNodeAssemblies
        {
            get
            {
                return Children.Cast<TestNodeAssembly>();
            }
        }
    }
}