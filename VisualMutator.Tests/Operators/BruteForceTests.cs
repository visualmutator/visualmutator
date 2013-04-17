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
                new ArithmeticOperatorReplacement(), 
             //   new AbsoluteValueInsertion(), 
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



        [Test]
        public void MutationSuccess()
        {
            //[ValueSource("_operators")]IMutationOperator oper
            var oper = new ArithmeticOperatorReplacement();

            List<MutMod> mutants;
            AssembliesProvider original;
          //  CodeDifferenceCreator diff;
            CommonCompilerAssemblies cci;
            CodeVisualizer visualizer;
            Common.RunMutationsReal(_assemblyPath, oper, out mutants, out original, out visualizer, out cci);
            //string fileTarget = Path.Combine(@"D:\PLIKI", original.Assemblies.First().Name.Value + ".dll");
            
        

            Assert.Pass();
        }
    }
}