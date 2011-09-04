namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System.Collections.ObjectModel;

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