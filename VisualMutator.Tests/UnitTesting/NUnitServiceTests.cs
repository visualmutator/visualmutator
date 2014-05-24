namespace VisualMutator.Tests.UnitTesting
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Model.Tests;
    using Model.Tests.Services;
    using Model.Tests.TestsTree;
    using Moq;
    using Ninject;
    using Ninject.Modules;
    using NUnit.Core;
    using NUnit.Framework;
    using Operators;
    using RunProcessAsTask;
    using UsefulTools.Core;
    using UsefulTools.Wpf;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    #endregion

    [TestFixture]
    public class NUnitServiceTests
    {
        private IKernel kernel;
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            var modules = new INinjectModule[]
            {
               // new VisualMutatorModule(), 
            };

            kernel = new StandardKernel();

            kernel.Load(modules);
            BasicConfigurator.Configure(
                new ConsoleAppender
                {
                    Layout = new SimpleLayout()
                });
        }

        #endregion

        private TestResult MockTestResult(string name, bool success)
        {
            var m = new Mock<ITest>();

            var tn = new TestName
                {
                    FullName = name,
                    Name = name
                };

            m.Setup(_ => _.TestName).Returns(tn);
            //   m.Object.

            var result = new TestResult(m.Object);

            if (success)
            {
                result.Success();
            }
            else
            {
                result.Error(new Exception("test exception"));
            }
            return result;
            /*
            var m = Mock.Of<ITest>(t =>
                t.TestName.UniqueName == name &&
                t.Tests.Add()

                );*/
        }

        [Test]
        public void Integration()
        {
            var m = new Mock<IMessageService>();
            var wrapper = new NUnitWrapper(m.Object);
            var service = new NUnitTestService(wrapper, m.Object);
            var session = new MutantTestSession();
           // service.LoadTests(new List<string> {MutationTestsHelper.DsaPath, MutationTestsHelper.DsaPath},
          //      session);

          //  service.RunTests(session);

            Thread.Sleep(15000);
           // Console.WriteLine(":session.TestMap.Count : " + session.TestsByAssembly.Count);
            
                
        }

        [Test]
        public void Loading2()
        {
            var nUnitWrapper = new NUnitWrapper(null);
            ITest loadTests = nUnitWrapper.LoadTests(new [] { @"C:\Users\Arego\AppData\Local\Microsoft\VisualStudio\12.0Exp\Extensions\PiotrTrzpil\VisualMutator\2.0.8\VisualMutator.dll" });
        Assert.Pass();
        }

        [Test]
        public void LoadingTests()
        {
            string nunitConsolePath =@"C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe";//"nunit-console",
            string outputFile = "nunit-results.xml";
            string arg = "\"" + MutationTestsHelper.DsaTestsPath + "\" /xml \"" + outputFile + "\" /nologo";
            

            var startInfo = new ProcessStartInfo
            {
                Arguments = arg,
                CreateNoWindow = true,
                ErrorDialog = true,
                RedirectStandardOutput = false,
                FileName = nunitConsolePath,
                UseShellExecute = false
            };

            Process proc = Process.Start(startInfo);
            bool res = proc.WaitForExit(1000 * 60);

            res = (res && proc.ExitCode >= 0);
        }


        private MutantTestResults GetResults(string assembly, SelectedTests selected)
        {
           // string path = @"C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe";
            var parser = new NUnitResultsParser();
            var context = new TestsRunContext();
            context.AssemblyPath = assembly;
            context.SelectedTests = selected;
           // var service = new NUnitTester(parser, kernel.Get<IProcesses>(), kernel.Get<CommonServices>(), path, context);

          //  service.RunTests().Wait();

            return context.TestResults;
        }


        [Test]
        public void ShouldRunAllTests()
        {
           


            MutantTestResults mutantTestResults = GetResults(MutationTestsHelper.DsaTestsPath,
                new SelectedTests(new List<TestId>(), ""));


            var c = mutantTestResults.ResultMethods.Count(r => r.State != TestNodeState.Success);
            
            //Assert.IsNotEmpty(results);
            Assert.AreEqual(1, c);
          //  var re = results.Values.Single(r => !r.Success);
           // Assert.AreNotEqual(re.StackTrace, "");
        }

        [Test]
        public void ShouldRunSelectedTest()
        {
            var tn = new TestName();
            tn.FullName = "Dsa.Test.DataStructures.SetTest.DuplicateObjectTest";            
            var tn2 = new TestName();
            tn2.FullName = "Dsa.Test.DataStructures.SetTest.ContainsTest";
            var selected = new SelectedTests(new List<TestId>
                              {
                                  new NUnitTestId(tn),
                                  new NUnitTestId(tn2)
                              }, "");



            MutantTestResults mutantTestResults = GetResults(MutationTestsHelper.DsaTestsPath, selected);


            var c = mutantTestResults.ResultMethods.Count(r => r.State != TestNodeState.Success);

           // Assert.IsNotEmpty(results);
            Assert.AreEqual(1, c);
         ////   Assert.AreEqual(2, results.Count);
          //  var re = results.Values.Single(r => !r.Success);
        //    Assert.AreNotEqual(re.StackTrace, "");
        }
        /*
        [Test]
        public void LoadingTests()
        {
            List<ITest> testClasses;
            var mock = TestWrapperMocking.MockNUnitWrapperForLoad(out testClasses);
            var clas = TestWrapperMocking.MockTestFixture("Class1", "ns1");
            clas.Tests.Add(TestWrapperMocking.MockTest("Test1", clas));
            clas.Tests.Add(TestWrapperMocking.MockTest("Test2", clas));
            clas.Tests.Add(TestWrapperMocking.MockTest("Test3", clas));
            testClasses.Add(clas);

            var ser = new NUnitTestService(mock.Object, new Mock<IMessageService>().Object);


            IEnumerable<TestNodeClass> testNodeClasses = ser.LoadTests(new Collection<string> { "a", "b" });


            var testFixture = testNodeClasses.Single();

            testFixture.Children.Count.ShouldEqual(3);
            testFixture.Children.ElementAt(2).Name.ShouldEqual("Test3");
        }


        [Test]
        public void RunningTests()
        {
            List<ITest> testClasses;
            var wrapperMock = TestWrapperMocking.MockNUnitWrapperForLoad(out testClasses);

            var clas = TestWrapperMocking.MockTestFixture("Class1", "ns1");
            clas.Tests.Add(TestWrapperMocking.MockTest("Test1", clas));
            clas.Tests.Add(TestWrapperMocking.MockTest("Test2", clas));
            clas.Tests.Add(TestWrapperMocking.MockTest("Test3", clas));
            testClasses.Add(clas);


            var list = new List<TestResult>
            {
                MockTestResult("Test1", true),
                MockTestResult("Test2", true),
                MockTestResult("Test3", false),
            };

            wrapperMock.Setup(_ => _.TestFinished).Returns(list.ToObservable());
            wrapperMock.Setup(_ => _.RunFinished).Returns(new List<TestResult> { new TestResult(new TestName()) }
                .ToObservable());


            
            var ser = new NUnitTestService(wrapperMock.Object, new Mock<IMessageService>().Object);
            var classes = ser.LoadTests(new Collection<string> { "a", "b" });


            ser.RunTests();

            ser.TestMap[list.ElementAt(0).Test.TestName.UniqueName].State.ShouldEqual(TestNodeState.Success);
            ser.TestMap[list.ElementAt(1).Test.TestName.UniqueName].State.ShouldEqual(TestNodeState.Success);
            ser.TestMap[list.ElementAt(2).Test.TestName.UniqueName].State.ShouldEqual(TestNodeState.Failure);

           classes.Single().State.ShouldEqual(TestNodeState.Failure);
        }

        */
    }
}