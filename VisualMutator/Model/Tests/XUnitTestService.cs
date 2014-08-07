namespace VisualMutator.Model.Tests
{
    using Services;
    using Strilanc.Value;

    public class XUnitTestService : ITestsService
    {
        public string FrameWorkName { get { return "XUnit"; } }

        public May<TestsLoadContext> LoadTests(string assemblyPath)
        {
            throw new System.NotImplementedException();
        }

        public void Cancel()
        {
            throw new System.NotImplementedException();
        }

        public TestsRunContext CreateRunContext(TestsLoadContext loadContext, string mutatedPath)
        {
            throw new System.NotImplementedException();
        }
    }
}