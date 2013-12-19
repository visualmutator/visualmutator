using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;


using Microsoft.Cci;

using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace VisualMutator.Console
{
    using System.Reflection;
    using System.Text;
    using Extensibility;
    using Infrastructure;
    using Model;
    using Model.Decompilation;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.Services;
    using Model.Tests.TestsTree;
    using Model.Verification;
    using Moq;
    using NUnit.Framework;
    using Tests.Util;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;
    using Console = System.Console;

    public class TestOperator2 : IMutationOperator
    {
        public class OperatorCodeVisitor2 : OperatorCodeVisitor
        {
            public List<Tuple<object, string>> objects = new List<Tuple<object, string>>();
            private int count;
            public override void VisitAny(object o)
            {
                count++;
                objects.Add(Tuple.Create(o, count.ToString()));
                MarkMutationTarget(o, count.ToString());

            }
        }
        public class OperatorCodeRewriter2 : OperatorCodeRewriter
        {
            private readonly OperatorCodeVisitor2 _operatorCodeVisitor2;

            public OperatorCodeRewriter2(OperatorCodeVisitor2 operatorCodeVisitor2)
            {
                _operatorCodeVisitor2 = operatorCodeVisitor2;
                
            }

            public override IExpression Rewrite(IExpression expre)
            {

                return expre;
            }
        }
        #region IMutationOperator Members

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("Test", "", "");
            }
        }

        public OperatorCodeVisitor2 visitor;
        public OperatorCodeRewriter2 rewriter2;
        public TestOperator2()
        {
            visitor = new OperatorCodeVisitor2();
            rewriter2 = new OperatorCodeRewriter2(visitor);
        }

        public IOperatorCodeVisitor CreateVisitor()
        {
            return visitor;
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return rewriter2;
        }

        #endregion
    }

    class Program
    {
        static string _assemblyPath = @"D:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa\bin\Debug\Dsa.dll";
        private static String file =
    @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa\bin\Debug\Dsa.dll";

        private static String file2 =
          @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\MiscUtil\MiscUtil\bin\Debug\MiscUtil.dll";
        
        private static void Main(string[] args)
        {
            var list = new List<string>
                       {
                           @"C:\PLIKI\Programowanie\awesome-19058\Tests\bin\Release\Omu.ValueInjecter.dll",
                           @"C:\PLIKI\Programowanie\awesome-19058\Tests\bin\Release\Omu.Awesome.Core.dll",
                           @"C:\PLIKI\Programowanie\awesome-19058\Tests\bin\Release\Omu.Awesome.Mvc.dll",
                           @"C:\PLIKI\Programowanie\awesome-19058\Tests\bin\Release\Omu.AwesomeDemo.Core.dll",
                           @"C:\PLIKI\Programowanie\awesome-19058\Tests\bin\Release\Omu.AwesomeDemo.Data.dll",
                           @"C:\PLIKI\Programowanie\awesome-19058\Tests\bin\Release\Omu.AwesomeDemo.Infra.dll",
                           @"C:\PLIKI\Programowanie\awesome-19058\Tests\bin\Release\Omu.AwesomeDemo.Service.dll",
                           @"C:\PLIKI\Programowanie\awesome-19058\Tests\bin\Release\Omu.AwesomeDemo.Tests.dll",
                           @"C:\PLIKI\Programowanie\awesome-19058\Tests\bin\Release\Omu.AwesomeDemo.WebUI.dll"
                       };
            var logMessageService = new LogMessageService();
            //    var mess = new Mock<IMessageService>();
            //  mess.Setup(_=>_.ShowError(It.IsAny<IMessageService>(), It.IsAny<string>(), It.IsAny<IWindow>()))
            var hostEnv = new Mock<IHostEnviromentConnection>();


            var fs = new FileSystemService();
            var cci = new CommonCompilerInfra();
            foreach (var path in list)
            {
                cci.AppendFromFile(path);
            }

            var mutantsContainer = new MutantsContainer(cci, new OperatorUtils(cci));
            var mutantCache = new MutantsCache(mutantsContainer);

            hostEnv.Setup(_ => _.GetProjectAssemblyPaths()).Returns(list.Select(_ => _.ToFilePathAbs()));
            hostEnv.Setup(_ => _.GetTempPath()).Returns(@"C:\PLIKI\Programowanie\Testy");

            FileManager fileManager = new FileManager(hostEnv.Object, new FileSystemService());
            MutantsFileManager mutantsFileManager = new MutantsFileManager(mutantCache, cci, fs);

            mutantsContainer.Initialize(new MutantsCreationOptions(), new List<TypeIdentifier>(), list.Select(_ => _.ToFilePathAbs()).ToList());


            TestEnvironmentInfo initTestEnvironment = fileManager.InitTestEnvironment(null);

            ExecutedOperator execOperator;
            Mutant changelessMutant = mutantsContainer.CreateEquivalentMutant(out execOperator);


            var testServ = new NUnitTestService(new NUnitWrapper(logMessageService), logMessageService);
            var mutantTestSession = new MutantTestSession();
            IEnumerable<TestNodeClass> testNodeClasses = testServ.LoadTests(list, mutantTestSession);

            var teco = new TestsContainer(testServ, mutantsFileManager, fileManager, new AssemblyVerifier());
            teco.InitTestEnvironment(new MutationTestingSession(initTestEnvironment));

            var storedMutantInfo = teco.StoreMutant(initTestEnvironment, changelessMutant);

            bool ddd = true;
            teco.RunTestsForMutant(new MutantsTestingOptions(), storedMutantInfo, changelessMutant);

            Assert.That(ddd, Is.True.After(5000));
            Assert.IsNotNull(testNodeClasses);
/*
            var cci = new CommonCompilerInfra();

            cci.AppendFromFile(file);
            cci.AppendFromFile(file2);
            var codeVisitor = new CodeVisitor();
            var visitor = new DebugOperatorCodeVisitor();
            var traverser = new DebugCodeTraverser(visitor);
            //
            var codeTrav = new CodeTraverser();
            codeTrav.PreorderVisitor = visitor;
            traverser.Traverse(cci.Modules);

            
            Console.WriteLine("ORIGINAL ObjectStructure:");
            string listing0 = visitor.ToString();
             //Console.WriteLine(listing0);
            File.WriteAllText("module11.txt", listing0, new UTF8Encoding());
            
            var cci2 = new CommonCompilerInfra();
            cci2.AppendFromFile(file);
            cci2.AppendFromFile(file2);

            var visitor2 = new DebugOperatorCodeVisitor();
            var traverser2 = new DebugCodeTraverser(visitor2);

            // traverser2.Traverse(cci.Copy(cci.Modules.Single()));
            traverser2.Traverse(cci2.Modules);
            File.WriteAllText("module22.txt", visitor2.ToString(), new UTF8Encoding());
        }

        static void Main2(string[] args)
        {


            BasicConfigurator.Configure(
           
              new ConsoleAppender
               {
                   Layout = new PatternLayout("%-5level - %-35.35logger{2} %-25.25method: %newline%message%newline%newline"),
                   Threshold = Level.Warn
               });
            var cci = new CommonCompilerInfra();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);

            cci.AppendFromFile(_assemblyPath);


            var original = new ModulesProvider(cci.Modules);

            ModulesProvider copiedModules = new ModulesProvider(
                cci.Modules.Select(cci.Copy).Cast<IModule>().ToList());


            ModulesProvider copiedModules1 = new ModulesProvider(
                cci.Modules.Select(cci.Copy).Cast<IModule>().ToList());


            ModulesProvider copiedModules2 = new ModulesProvider(
          cci.Modules.Select(cci.Copy).Cast<IModule>().ToList());

            var commonTargets = new List<MutationTarget>();
            var oper = new TestOperator2();
            var ded = oper.CreateVisitor();
            IOperatorCodeVisitor operatorVisitor = ded;
            operatorVisitor.Host = cci.Host;
            operatorVisitor.OperatorUtils = utils;
            operatorVisitor.Initialize();
            var mergedTargets = new List<Tuple<string , List<MutationTarget>>>();

            var visitor = new VisualCodeVisitor(operatorVisitor);

            var traverser = new VisualCodeTraverser(new List<TypeIdentifier>(), visitor);

            traverser.Traverse(copiedModules2.Assemblies.Single());
            visitor.PostProcess();
            IEnumerable<Tuple<string, List<MutationTarget>>> s = visitor.MutationTargets.AsEnumerable();
            mergedTargets.AddRange(s);

            commonTargets.AddRange(visitor.SharedTargets);
            var mutargets = visitor.MutationTargets.Select(t => t.Item2).Flatten().ToList();

            var visitorBack = new VisualCodeVisitorBack(mutargets, new List<MutationTarget>());
            var traverser2 = new VisualCodeTraverser(new List<TypeIdentifier>(), visitorBack);
            traverser2.Traverse(copiedModules1.Assemblies.Single());
            visitorBack.PostProcess();*/
        }
    }
}
