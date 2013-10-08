namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommonUtilityInfrastructure.Paths;
    using Extensibility;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using NUnit.Framework;
    using OperatorsStandard;
    using Util;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;

    [TestFixture]
    public class BruteForceTests
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
            var m = Common.CreateModules(code);
                Assert.Pass();
        }
     
    }
}