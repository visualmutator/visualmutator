namespace VisualMutator.Tests.Util
{
    using System.Reflection;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Model.Tests;
    using Model.Tests.Services;
    using Ninject;
    using NUnit.Framework;
    using VisualMutator.Infrastructure;

   // [TestFixture]
    public class IntegrationTest
    {
        protected static ILog _log;
        protected StandardKernel _kernel;

        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure(
                new ConsoleAppender
            {
                Layout = new SimpleLayout()
            });
            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            _kernel = new StandardKernel();
            _kernel.Load(new IntegrationTestModule());

        }



    }
}