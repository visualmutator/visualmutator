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
    public class NinjectParamBindingsTests
    {
        private StandardKernel _kernel;

        [Test]
        public void Test1()
        {
          
        }

        [Test]
        public void TestBindingParameters()
        {
            var modules = new INinjectModule[]
            {
                new TestModuleChilds(), 
            };

            _kernel = new StandardKernel();
            _kernel.Components.Add<IActivationStrategy, MyMonitorActivationStrategy>();

            _kernel.Load(modules);

            var factory = _kernel.Get<IBindingFactory<SomeTimedModule>>();
            SomeTimedModule someTimedModule1 = factory.CreateWithBindings(new SomeObject(3));
            Assert.AreEqual(someTimedModule1.SomeObject.Parameter, 
                someTimedModule1.InnerModule.SomeObject.Parameter);
        }
        
        public class TestModuleChilds : NinjectModule
        {
            public override void Load()
            {
                Kernel.InjectChildFactory<SomeTimedModule>(childKernel =>
                {
                    childKernel.Bind<SomeInnerModule>().ToSelf().InSingletonScope();

                });
                
                Kernel.Bind<SomeObject>().ToSelf().AndFromFactory();
            }
        }
        public class SomeInnerModule
        {
            public SomeObject SomeObject { get; set; }

            public SomeInnerModule(SomeObject someObject)
            {
                SomeObject = someObject;
            }
        }
        public class SomeTimedModule
        {
            public SomeInnerModule InnerModule { get; set; }
            public SomeObject SomeObject { get; set; }

            public SomeTimedModule(
                SomeInnerModule innerModule,
                SomeObject someObject)
            {
                InnerModule = innerModule;
                SomeObject = someObject;
            }
        }
      
        public class SomeObject
        {
            private readonly int _parameter;

            public SomeObject( int parameter)
            {
                _parameter = parameter;
            }
            public int Parameter
            {
                get { return _parameter; }
            }
        }
      
        public class SomeTimedObjectInner
        {
            public static int i = 0;
            public int id;

            public SomeTimedObjectInner()
            {
                id = i++;
            }

        }
        public class SomeTimedObjectInnerInner
        {
            public SomeTimedObjectInner Inner { get; set; }
            public static int i = 0;
            public int id;

            public SomeTimedObjectInnerInner(SomeTimedObjectInner inner)
            {
                Inner = inner;
                id = i++;
            }
        }
    }
}