namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests.TestsTree;

    public class TestSession : ModelElement
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

        public string ErrorMessage
        {
            get;
            set;
        }

        public string ErrorDescription
        {
            get;
            set;
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

        private bool _isComplete;

        public bool IsComplete
        {
            get
            {
                return _isComplete;
            }
            set
            {
                SetAndRise(ref _isComplete, value, () => IsComplete);
            }
        }
    

        public long RunTestsTimeRawMiliseconds { get; set; }

        public long LoadTestsTimeRawMiliseconds { get; set; }

        public Exception Exception { get; set; }
    }
}