namespace VisualMutator.Model.Tests
{
    using NUnit.Core;

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