namespace VisualMutator.Model.Tests
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TestsTree;
    using UsefulTools.Core;

    #endregion

    public class MutantTestSession : ModelElement
    {
        private IDictionary<string, TestNodeMethod> _testMap;

    


        private TestsRootNode _testsRootNode;

        public MutantTestSession()
        {
            _testMap = new Dictionary<string, TestNodeMethod>();
            
        
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