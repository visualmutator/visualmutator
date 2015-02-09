namespace VisualMutator.Tests.Infrastructure
{
    #region

    using Ninject;
    using Ninject.Activation.Strategies;
    using Ninject.Modules;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.DependencyInjection;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    #endregion

    [TestFixture]
    public class NinjectMemoryTests
    {
        private StandardKernel _kernel;

        [Test]
        public void Test1()
        {
          
        }

        [Test]
        public void TestPartialParameters()
        {
            var modules = new INinjectModule[]
            {
                new TestModule(), 
            };

            _kernel = new StandardKernel();
            _kernel.Components.Add<IActivationStrategy, MyMonitorActivationStrategy>();

            _kernel.Load(modules);

            var factory = _kernel.Get<IFactory<SomeMainModule>>();
            for (int i = 0; i < 1000000; i++)
            {
                factory.Create();
            }
        }

        public class TestModule : NinjectModule
        {
            public override void Load()
            {
                Kernel.BindObjectRoot<SomeMainModule>().ToSelf(k =>
                {
                    k.Bind<SomeMainModule>().ToSelf().InSingletonScope();
                    k.Bind<SomeObject>().ToSelf().AndFromFactory();
                });
            }
        }
      
        public class SomeMainModule
        {
            private readonly IFactory<SomeObject> _objectFactory;
            private SomeObject someObject;

            public SomeMainModule(IFactory<SomeObject> objectFactory)
            {
                _objectFactory = objectFactory;
                someObject = _objectFactory.Create();
            }
        }
        public class SomeObject
        {
            private readonly SomeMainModule _module;
            private int[] _bigArray;

            public SomeObject(SomeMainModule module)
            {
                _module = module;
                _bigArray = new int[5000];
            }
        }
    }
}