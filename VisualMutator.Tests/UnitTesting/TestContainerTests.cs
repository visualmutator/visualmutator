namespace VisualMutator.Tests.UnitTesting
{
    using System.Collections.Generic;
    using System.Linq;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;

    using Mono.Cecil;

    using Moq;

    using NUnit.Core;
    using NUnit.Framework;

    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class TestContainerTests
    {

        [Test]
        public void Test1()
        {
            List<ITest> testClasses;
            var nUnitWrapperMock = TestWrapperMocking.MockNUnitWrapperForLoad(out testClasses);

            var clas = TestWrapperMocking.MockTestFixture("Class1", "ns1");
            clas.Tests.Add(TestWrapperMocking.MockTest("Test1", clas));
            clas.Tests.Add(TestWrapperMocking.MockTest("Test2", clas));
            clas.Tests.Add(TestWrapperMocking.MockTest("Test3", clas));
            testClasses.Add(clas);



            var td = new TypeDefinition("ns1", "Class2", TypeAttributes.Public);

            td.Methods.Add(CecilUtils.CreateMethodDefinition("Test1", td));
            td.Methods.Add(CecilUtils.CreateMethodDefinition("Test2", td));
            td.Methods.Add(CecilUtils.CreateMethodDefinition("Test3", td));
            //  AssemblyDefinition assembly = TestWrapperMocking.CreateAssembly("Ass", new[] { td });

            //  var mock = TestWrapperMocking.MockMsTestWrapperForLoad(out testMethods);

            var msTestLoaderMock = new Mock<IMsTestLoader>();
            msTestLoaderMock.Setup(_ => _.ScanAssemblies(It.IsAny<IEnumerable<string>>())).Returns(new AssemblyScanResult
            {
                AssembliesWithTests = new[] { "Ass" },
                TestMethods = td.Methods
            });




            var nUnitTestService = new NUnitTestService(nUnitWrapperMock.Object, new Mock<IMessageService>().Object);
            var msTestService = new MsTestService(null, msTestLoaderMock.Object);
       
            var container = new TestsContainer(nUnitTestService, msTestService, null,Create.TestServices());
         
            var mutant = new StoredMutantInfo();
            mutant.AssembliesPaths.Add( "a");
       
            //Act:
            TestSession session = container.LoadTests(mutant);


            //Assert:
            TestNodeNamespace ns = (TestNodeNamespace)session.TestsRootNode.Children.Single();
            ns.Children.Count.ShouldEqual(2);
            
            Assert.IsNotNull(ns.Parent);

            ns.Children.Each(_ => _.Parent.ShouldEqual(ns));



            ns.State.ShouldEqual(TestNodeState.Inactive);
            ns.Children.Cast<TestTreeNode>().Each(_ => _.State.ShouldEqual(TestNodeState.Inactive));
            ns.Children.Cast<TestTreeNode>().Each(c => c.Children.Cast<TestTreeNode>().Each(_ => _.State.ShouldEqual(TestNodeState.Inactive)));



            session.TestsRootNode.State = TestNodeState.Running;
            Assert.IsTrue(ReferenceEquals(session.TestsRootNode.Children.Single(), ns));
            

            ns.State.ShouldEqual(TestNodeState.Running);
            ns.Children.Cast<TestTreeNode>().Each(_ => _.State.ShouldEqual(TestNodeState.Running));
            ns.Children.Cast<TestTreeNode>().Each(c => c.Children.Cast<TestTreeNode>()
                .Each(_ => _.State.ShouldEqual(TestNodeState.Running)));




        }
    }
}

