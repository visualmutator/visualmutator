namespace VisualMutator.Tests.UnitTesting
{
    #region

    using Model.Tests;
    using Model.Tests.Services;
    using Ninject;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;
    using Strilanc.Value;
    using Util;
    using VisualMutator.Infrastructure;

    #endregion

    [TestFixture]
    public class XUnitServiceTests : IntegrationTest
    {


//        [SetUp]
//        public void Setup()
//        {
//            _kernel = new StandardKernel();
//            _kernel.Load(new IntegrationTestModule());
//            _kernel.Bind<XUnitTestsRunContext>().ToSelf().AndFromFactory();
//            _kernel.Bind<XUnitTestService>().ToSelf();
//        }


        [Test]
        public void LoadTests()
        {
            _kernel.Bind<XUnitTestsRunContext>().ToSelf().AndFromFactory();
            _kernel.Bind<XUnitTestService>().ToSelf();
            var service = _kernel.Get<XUnitTestService>();
            var loadCtx = service.LoadTests(TestProjects.AutoMapper).ForceGetValue();
            foreach (var ns in loadCtx.Namespaces)
            {
                ns.IsIncluded = true;
            }
            loadCtx.ClassNodes.Count.ShouldBeGreaterThan(0);

            var runCtx = service.CreateRunContext(loadCtx, TestProjects.AutoMapper);
            var results = runCtx.RunTests().Result;
            results.ResultMethods.Count.ShouldBeGreaterThan(0);
//            foreach (var tmpTestNodeMethod in results.ResultMethods)
//            {
//                _log.Debug("Test: "+ tmpTestNodeMethod.Name+ " state: "+ tmpTestNodeMethod.State);
//            }
        }

    }
}