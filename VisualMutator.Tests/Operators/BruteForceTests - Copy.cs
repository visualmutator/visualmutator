namespace VisualMutator.Tests.Operators
{
    #region

    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class IntegrationTests
    {
  

        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
       
            BasicConfigurator.Configure(
                new ConsoleAppender
                    {
                        Layout = new SimpleLayout()
                    });
        }

        #endregion

        const string code =
    @"using System;
namespace Ns
{
    public class Test
    {
    
        public bool Method1(int x)
        {
            return x != 1;
        }
        public bool Method1(int x, int y)
        {
            return Method1(x);
        }
    }
}";

        [Test]
        public void MutationSuccess()
        {
           
        }
     
    }
}