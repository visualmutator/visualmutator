namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;
    using TestsTree;

    public class TestsLoadContext
    {
        private readonly Dictionary<string /*testName*/, TestNodeMethod> _testMap;
        private readonly List<TestNodeClass> _classNodes;

        public TestsLoadContext()
        {
            _testMap = new Dictionary<string, TestNodeMethod>();
            _classNodes = new List<TestNodeClass>();
        }

        public string AssemblyPath { get; set; }


        public List<TestNodeClass> ClassNodes
        {
            get { return _classNodes; }
        }

    }
}