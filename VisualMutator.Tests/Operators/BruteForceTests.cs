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
    using Model.Mutations.MutantsTree;
    using NUnit.Framework;
    using OperatorsStandard;
    using Util;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;

    [TestFixture]
    public class BruteForceTests
    {
        private string _assemblyPath;
        private string _targetPath;
        private IMutationOperator[] _operators;

        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _assemblyPath = @"D:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa\bin\Debug\Dsa.dll";
            _targetPath = Path.Combine(@"D:\PLIKI",Path.GetFileName(_assemblyPath));

            _operators = new IMutationOperator[]
            {
                new AOR_ArithmeticOperatorReplacement(), 
             //   new ABS_AbsoluteValueInsertion(), 
            };
/*
            new TestCaseData(0, 0)
    .Throws(typeof(DivideByZeroException))
    .SetName("DivideByZero")
    .SetDescription("An exception is expected");*/
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