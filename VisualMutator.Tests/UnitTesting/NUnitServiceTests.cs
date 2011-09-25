namespace VisualMutator.Tests.UnitTesting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Collections.Generic;

    using Moq;

    using NUnit.Core;
    using NUnit.Framework;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;

    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class NUnitServiceTests
    {

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



    }
}

