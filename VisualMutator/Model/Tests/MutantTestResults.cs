namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;

    public class MutantTestResults
    {
        private readonly bool _cancelled;
        public List<TmpTestNodeMethod> ResultMethods { get; private set; }

        public bool Cancelled
        {
            get { return _cancelled; }
        }

        public MutantTestResults(List<TmpTestNodeMethod> result)
        {
            ResultMethods = result;
        }
        public MutantTestResults(bool cancelled = false)
        {
            _cancelled = cancelled;
            ResultMethods = new List<TmpTestNodeMethod>();
        }
    }
}