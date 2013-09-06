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

        private String file =
            @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa\bin\Debug\Dsa.dll";

        private String file2 =
          @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\MiscUtil\MiscUtil\bin\Debug\MiscUtil.dll";
        
        [Test]
        public void MutationSuccess()
        {
            var m = Common.CreateModules(code);
                Assert.Pass();
        }
        [Test]
        public void DebugTraverse()
        {

            var cci = new CommonCompilerAssemblies();
           
            cci.AppendFromFile(file);
            cci.AppendFromFile(file2);

            var visitor = new DebugOperatorCodeVisitor();
            var traverser = new DebugCodeTraverser(visitor);

            traverser.Traverse(cci.Modules);

            Console.WriteLine("ORIGINAL ObjectStructure:");
            string listing0 = visitor.ToString();
           // Console.WriteLine(listing0);
            File.WriteAllText("module11.txt",listing0);

            var cci2 = new CommonCompilerAssemblies();
            cci2.AppendFromFile(file);
            cci2.AppendFromFile(file2);

            var visitor2 = new DebugOperatorCodeVisitor();
            var traverser2 = new DebugCodeTraverser(visitor2);

           // traverser2.Traverse(cci.Copy(cci.Modules.Single()));
            traverser2.Traverse(cci2.Modules);
            File.WriteAllText("module22.txt", visitor2.ToString());
          //  Common.DebugTraverseFile();
            //var m = Common.CreateModules(code);
           //     Assert.Pass();
        }
    }
}