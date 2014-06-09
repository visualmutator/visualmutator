namespace VisualMutator.Tests.Mutations
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Model.StoringMutants;
    using Moq;
    using Ninject;
    using Ninject.Modules;
    using Ninject.Syntax;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.Paths;
    using Util;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    [TestFixture]
    public class WhiteCacheIntTests
    {
        private StandardKernel _kernel;


        [SetUp]
        public void Setup()
        {
            _kernel = new StandardKernel();
            _kernel.Load(new IntegrationTestModule());
        }

        [Test]
        public void Test()
        {
            var paths = new[] {
                 TestProjects.DsaPath,
                 TestProjects.DsaTestsPath,
                 TestProjects.DsaTestsPath2}.Select(_ => new FilePathAbsolute(_)).ToList();

            _kernel.Bind<IProjectClonesManager>().To<ProjectClonesManager>().InSingletonScope();
            _kernel.Bind<ProjectFilesClone>().ToSelf().AndFromFactory();
            _kernel.Bind<FilesManager>().ToSelf().InSingletonScope();
            _kernel.Bind<WhiteCache>().ToSelf().AndFromFactory();
            _kernel.BindMock<IHostEnviromentConnection>(mock =>
            {
                mock.Setup(_ => _.GetProjectAssemblyPaths()).Returns(paths);
                mock.Setup(_ => _.GetTempPath()).Returns(Path.GetTempPath());
            });

            var cache = _kernel.GetFromFactory<WhiteCache>(2);
            cache.Initialize().Wait();

            var a = cache.GetWhiteModulesAsync("Dsa.Test").Result;
            var b = cache.GetWhiteModulesAsync("Dsa").Result;
            var c = cache.GetWhiteModulesAsync("Dsa").Result;

            a.Modules.Count.ShouldEqual(1);
            b.Modules.Count.ShouldEqual(1);
            c.Modules.Count.ShouldEqual(1);
        }



    }


    public static class NinjectExt
    {
        public static void BindMock<TMock>(this KernelBase kernel, Action<Mock<TMock> > mockSetup ) where TMock : class
        {
            var mock = new Mock<TMock>();
            mockSetup(mock);
            kernel.Bind<TMock>().ToConstant(mock.Object);
        }

        public static T GetFromFactory<T>(this KernelBase kernel, params object[] pars)
        {
            return kernel.Get<IBindingFactory<T>>().CreateWithParams(pars);
        }
    }
}