namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;
    using TestsTree;

    public class TestsLoadContext
    {
        private readonly Dictionary<string /*testName*/, TestNodeMethod> _testMap;
        private readonly List<TestNodeClass> _classNodes;
        public TestNodeAssembly TestNodeAssembly { get; set; }

        public TestsLoadContext()
        {
            _testMap = new Dictionary<string, TestNodeMethod>();
            _classNodes = new List<TestNodeClass>();
        }

        public IDictionary<string, TestNodeMethod> TestMap
        {
            get
            {
                return _testMap;
            }
        }

        public List<TestNodeClass> ClassNodes
        {
            get { return _classNodes; }
        }

        public SelectedTests SelectedTests { get; set; }
    }
}