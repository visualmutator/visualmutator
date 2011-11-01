namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using VisualMutator.Model.Tests.TestsTree;

    public class TestSession
    {
        private IDictionary<string, TestNodeMethod> _testMap;

        private List<TestNodeClass> _testClassses;


        private TestsRootNode _testsRootNode;

        public TestSession()
        {
            _testMap = new Dictionary<string, TestNodeMethod>();
            _testClassses = new List<TestNodeClass>();
        
            _testsRootNode = new TestsRootNode();
        }

        public IDictionary<string, TestNodeMethod> TestMap
        {
            get
            {
                return _testMap;
            }
        }

        public List<TestNodeClass> TestClassses
        {
            get
            {
                return _testClassses;
            }
        }

        public long TestingTimeMiliseconds
        {
            get;
            set;
        }
        public TestsRootNode TestsRootNode
        {
            get
            {
                return _testsRootNode;
            }
        }

        public IEnumerable<TestNodeNamespace> TestNamespaces
        { 
            get
            {
                return TestsRootNode.Children.Cast<TestNodeNamespace>();
            } 
        }

        public IList<string> AssembliesWithTests { get; set; }

        public bool IsComplete
        {
            get; set; }
    }
}