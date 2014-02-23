namespace VisualMutator.Model.Tests
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Services;
    using TestsTree;
    using UsefulTools.Core;

    #endregion

    public class MutantTestSession : ModelElement
    {
        private IDictionary<string, TestsLoadContext> _testsByAssembly;

        private TestsRootNode _testsRootNode;

        public MutantTestSession()
        {
            _testsByAssembly = new Dictionary<string, TestsLoadContext>();
            
        
            _testsRootNode = new TestsRootNode();
        }

//        public IDictionary<string, TestsLoadContext> TestsByAssembly
//        {
//            get
//            {
//                return _testsByAssembly;
//            }
//        }

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
//        public TestsRootNode TestsRootNode
//        {
//            get
//            {
//                return _testsRootNode;
//            }
//        }

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