namespace VisualMutator.Tests.UnitTesting
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensibility;
    using Model;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.Services;
    using Model.Tests.TestsTree;
    using Model.Verification;
    using Moq;
    using NUnit.Framework;
    using Strilanc.Value;
    using UsefulTools.Core;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;
    using Util;
    using VisualMutator.Infrastructure;

    #endregion

    [TestFixture]
    public class TestContainerTests
    {
        public TestContainerTests(SelectedTests selectedTests)
        {
            _selectedTests = selectedTests;
        }

        private SelectedTests _selectedTests;

        [SetUp]
        public void TestSetUp()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [Test]
        public void TestTreeBuilding()
        {
           
        }

        /*
        [Test]
        public void Test1()
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
            var dispMock = new Mock<IDispatcherExecute>();
            dispMock.Setup(_ => _.GuiScheduler).Returns(TaskScheduler.FromCurrentSynchronizationContext());
            
            var fs = new FileSystemService();
            var cci = new CciModuleSource();
            foreach (var path in list)
            {
                cci.AppendFromFile(path);
            }
            
            var mutantsContainer = new MutantsContainer(cci, new OperatorUtils(cci));
            var mutantCache = new MutantsCache(mutantsContainer);

            hostEnv.Setup(_ => _.GetProjectAssemblyPaths()).Returns(list.Select(_ => _.ToFilePathAbs()));
            hostEnv.Setup(_ => _.GetTempPath()).Returns(@"C:\PLIKI\Programowanie\Testy");

            var fileManager = new FileSystemManager(hostEnv.Object, new FileSystemService());
            MutantsFileManager mutantsFileManager = new MutantsFileManager(mutantCache, cci, fs);

            mutantsContainer.Initialize(new List<IMutationOperator>(), 
                new MutantsCreationOptions(), MutationFilter.AllowAll());


            var clone = fileManager.CreateClone();

            AssemblyNode execOperator;
            Mutant changelessMutant = mutantsContainer.CreateEquivalentMutant(out execOperator);


            var testServ = new NUnitXmlTestService(new NUnitWrapper(logMessageService), new NUnitExternal(null, null), null);
            var mutantTestSession = new MutantTestSession();
            May<TestsLoadContext> loadTests = testServ.LoadTests(list);

            var teco = new TestsContainer(testServ, mutantsFileManager, fileManager, new AssemblyVerifier());
            teco.InitTestEnvironment(new MutationTestingSession(clone));

            var storedMutantInfo = teco.StoreMutant(clone, changelessMutant);

            bool ddd = true;
            teco.RunTestsForMutant(new MutantsTestingOptions(), storedMutantInfo, changelessMutant, _selectedTests);

            Assert.That(ddd, Is.True.After(5000));
            Assert.IsNotNull(loadTests);
        }

         [Test]
        public void Test1()
        {
            List<ITest> testClasses;
            Mock<INUnitWrapper> nUnitWrapperMock = TestWrapperMocking.MockNUnitWrapperForLoad(out testClasses);

            ITest clas = TestWrapperMocking.MockTestFixture("Class1", "ns1");
            clas.Tests.Add(TestWrapperMocking.MockTest("Test1", clas));
            clas.Tests.Add(TestWrapperMocking.MockTest("Test2", clas));
            clas.Tests.Add(TestWrapperMocking.MockTest("Test3", clas));
            testClasses.Add(clas);


            var td = new TypeDefinition("ns1", "Class2", TypeAttributes.Public);

            td.Methods.Add(CecilUtils.CreateMethodDefinition("Test1", td));
            td.Methods.Add(CecilUtils.CreateMethodDefinition("Test2", td));
            td.Methods.Add(CecilUtils.CreateMethodDefinition("Test3", td));


            var msTestLoaderMock = new Mock<IMsTestLoader>();
            msTestLoaderMock.Setup(_ => _.ScanAssemblies(It.IsAny<IEnumerable<string>>())).Returns(new AssemblyScanResult
                {
                    AssembliesWithTests = new[] {"Ass"},
                    //TODO: TestMethods = td.Methods
                });


            var nUnitTestService = new NUnitTestService(nUnitWrapperMock.Object, new Mock<IMessageService>().Object);
            var msTestService = new MsTestService(null, msTestLoaderMock.Object);

            //var container = new TestsContainer(nUnitTestService, msTestService, null,Create.TestServices(),null);

            var mutant = new StoredMutantInfo();
            mutant.AssembliesPaths.Add("a");

            //Act:
            var session = new MutantTestSession();

            //container.LoadTests(mutant, session);


            //Assert:
            var ns = (TestNodeNamespace) session.TestsRootNode.Children.Single();
            ns.Children.Count.ShouldEqual(2);

            Assert.IsNotNull(ns.Parent);

            ns.Children.Each(_ => _.Parent.ShouldEqual(ns));


            ns.State.ShouldEqual(TestNodeState.Inactive);
            ns.Children.Cast<TestTreeNode>().Each(_ => _.State.ShouldEqual(TestNodeState.Inactive));
            ns.Children.Cast<TestTreeNode>().Each(
                c => c.Children.Cast<TestTreeNode>().Each(_ => _.State.ShouldEqual(TestNodeState.Inactive)));


            session.TestsRootNode.State = TestNodeState.Running;
            Assert.IsTrue(ReferenceEquals(session.TestsRootNode.Children.Single(), ns));


            ns.State.ShouldEqual(TestNodeState.Running);
            ns.Children.Cast<TestTreeNode>().Each(_ => _.State.ShouldEqual(TestNodeState.Running));
            ns.Children.Cast<TestTreeNode>().Each(c => c.Children.Cast<TestTreeNode>()
                                                           .Each(_ => _.State.ShouldEqual(TestNodeState.Running)));
        }*/
    }
}