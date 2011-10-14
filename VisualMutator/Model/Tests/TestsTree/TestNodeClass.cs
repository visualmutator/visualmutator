namespace VisualMutator.Model.Tests.TestsTree
{
    #region Usings

    

    #endregion

    public class TestNodeClass : TestTreeNode
    {
       
        public string Namespace { get; set; }


        public TestNodeClass(string name)
            : base(null, name, true)
        {
           
        }


        public string FullName { get; set; }
    }

    public class TestsRootNode : TestTreeNode
    {
        public TestsRootNode()
            : base(null, "", true)
        {

        }
    }
}