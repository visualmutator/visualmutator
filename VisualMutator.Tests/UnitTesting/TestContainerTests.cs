namespace VisualMutator.Tests.UnitTesting
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    using Moq;

    using NUnit.Core;
    using NUnit.Framework;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Tests;

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

            td.Methods.Add(TestWrapperMocking.CreateMethodDefinition("Test1", td));
            td.Methods.Add(TestWrapperMocking.CreateMethodDefinition("Test2", td));
            td.Methods.Add(TestWrapperMocking.CreateMethodDefinition("Test3", td));
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

            var container = new TestsContainer(nUnitTestService, msTestService);



            var namespaces = container.LoadTests(new[]{"a"});

            var ns = namespaces.Single();
            ns.Children.Count.ShouldEqual(2);
            
            Assert.IsNotNull(ns.Parent);

            ns.Children.Each(_ => _.Parent.ShouldEqual(ns));



            ns.State.ShouldEqual(TestNodeState.Inactive);
            ns.Children.Cast<TestTreeNode>().Each(_ => _.State.ShouldEqual(TestNodeState.Inactive));
            ns.Children.Cast<TestTreeNode>().Each(c => c.Children.Cast<TestTreeNode>().Each(_ => _.State.ShouldEqual(TestNodeState.Inactive)));



            container.TestsRootNode.State = TestNodeState.Running;
            Assert.IsTrue(ReferenceEquals(container.TestsRootNode.Children.Single(), ns));
            

            ns.State.ShouldEqual(TestNodeState.Running);
            ns.Children.Cast<TestTreeNode>().Each(_ => _.State.ShouldEqual(TestNodeState.Running));
            ns.Children.Cast<TestTreeNode>().Each(c => c.Children.Cast<TestTreeNode>()
                .Each(_ => _.State.ShouldEqual(TestNodeState.Running)));




        }
    }
}

