namespace VisualMutator.Model.Tests.TestsTree
{
    public class TestNodeMethod : TestTreeNode
    {
        public TestNodeMethod(TestNodeClass parent, string name)
            : base(parent, name, false)
        {
        }
        public TestNodeClass ParentClass
        {
            get
            {
                return (TestNodeClass)Parent;
            }
        }
   
    }
}