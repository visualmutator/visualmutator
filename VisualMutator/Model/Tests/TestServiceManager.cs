namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Services;
    using Strilanc.Value;

    public class TestServiceManager
    {

        private readonly Dictionary<string, ITestsService> _services;

        public TestServiceManager(
            NUnitXmlTestService nunit)
         //   XUnitTestService xunit)
        {
            _services = new List<ITestsService>
                        {
                            nunit,
                         //   xunit
                        }.ToDictionary(s => s.FrameWorkName);

        }

        public async Task<List<TestsLoadContext>>  LoadTests(string assemblyPath)
        {
            var r = await Task.WhenAll(_services.Values.Select(s => Task.Run(() => s.LoadTests(assemblyPath))));
            return r.Where(m => m.HasValue).Select(m => m.ForceGetValue()).ToList();
              
        }

        public TestsRunContext CreateRunContext(TestsLoadContext loadContext, string mutatedPath)
        {
            return _services[loadContext.FrameworkName].CreateRunContext(loadContext, mutatedPath);
        }
    }
}