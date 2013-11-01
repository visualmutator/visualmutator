namespace VisualMutator.Model.Tests
{
    #region

    using NUnit.Core;

    #endregion

    public abstract class TestId
    {
         
    }
    public class NUnitTestId : TestId
    {
        public TestName TestName { get; set; }

        public NUnitTestId(TestName testName)
        {
            TestName = testName;
        }
    }
    public class MsTestTestId : TestId
    {

    }
}