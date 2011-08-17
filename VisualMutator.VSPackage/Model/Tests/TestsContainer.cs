namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TestsContainer
    {
        private IDictionary<string, TestTreeNode> _testMap;

        public TestsContainer()
        {

            _testMap = new Dictionary<string, TestTreeNode>();
             
        }
    }
}