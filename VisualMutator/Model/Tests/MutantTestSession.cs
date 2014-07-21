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

        public MutantTestSession()
        {
        
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
        public DateTime TestingEnd { get; set; }
    }
}