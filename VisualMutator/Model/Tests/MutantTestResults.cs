namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;

    public class MutantTestResults
    {
        public List<TmpTestNodeMethod> ResultMethods { get; set; }

        public MutantTestResults(List<TmpTestNodeMethod> result)
        {
            ResultMethods = result;
        }
    }
}