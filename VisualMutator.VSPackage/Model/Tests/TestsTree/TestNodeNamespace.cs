namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System.Collections.ObjectModel;

    #endregion

    public class TestNodeNamespace : TestTreeNode
    {
     
        public TestNodeNamespace(TestsRootNode parent, string name)
            : base(parent, name, true)
        {
        }

    }
}