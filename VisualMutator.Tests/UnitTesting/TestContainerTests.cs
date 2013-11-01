namespace VisualMutator.Tests.UnitTesting
{
    #region

    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestContainerTests
    {
       /* [Test]
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