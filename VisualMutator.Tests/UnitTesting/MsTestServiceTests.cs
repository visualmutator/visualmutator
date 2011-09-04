namespace VisualMutator.Tests.UnitTesting
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Mono.Cecil;
    using Mono.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Tests;

    using VisualMutator.Tests.Util;

    [TestFixture]
    public class MsTestServiceTests
    {
      

        private void LoadTests()
        {
            
        }


        [Test]
        public void LoadingTests()
        {
    
           

            List<MethodDefinition> testMethods;
            var mock = TestWrapperMocking.MockMsTestWrapperForLoad(out testMethods);
            var td = new TypeDefinition("ns2", "Class1", TypeAttributes.Public);
            testMethods.Add(TestWrapperMocking.CreateMethodDefinition("Test1", td));
            testMethods.Add(TestWrapperMocking.CreateMethodDefinition("Test2", td));
            testMethods.Add(TestWrapperMocking.CreateMethodDefinition("Test3", td));


            var ser = new MsTestService(mock.Object, new Mock<IMessageService>().Object);


            IEnumerable<TestNodeClass> testNodeClasses = ser.LoadTests(new Collection<string> { "a"});


            var testFixture = testNodeClasses.Single();

            testFixture.Children.Count.ShouldEqual(3);
            testFixture.Children.ElementAt(1).Name.ShouldEqual("Test2");
            testFixture.Children.ElementAt(2).Name.ShouldEqual("Test3");
        }

        [Test]
        public void RunningTests()
        {
        

            var d = 
                new XDocument(

                    new XElement("TestingResults",
                        new XElement("UnitTest",
                            new XAttribute("id", "id1"),
                            new XElement("TestMethod",
                                new XAttribute("name", "Test1"),
                                new XAttribute("className", "ns2.Class1,assembly1")
                            )
                        ),
                        new XElement("UnitTest",
                            new XAttribute("id", "id2"),
                            new XElement("TestMethod",
                                new XAttribute("name", "Test2"),
                                new XAttribute("className", "ns2.Class1,assembly1")
                            )
                        ),
                        new XElement("UnitTestResult",
                            new XAttribute("testId", "id1"),
                            new XAttribute("outcome", "Passed")
                        ),
                        new XElement("UnitTestResult",
                            new XAttribute("testId", "id2"),
                            new XAttribute("outcome", "Failed"),
                            new XElement("ErrorInfo",
                                new XElement("Message", "Error message")
                            )
                        )
                    )
               );



            List<MethodDefinition> testMethods;
            var mock = TestWrapperMocking.MockMsTestWrapperForLoad(out testMethods);
            var td = new TypeDefinition("ns2", "Class1", TypeAttributes.Public);
            testMethods.Add(TestWrapperMocking.CreateMethodDefinition("Test1", td));
            testMethods.Add(TestWrapperMocking.CreateMethodDefinition("Test2", td));
            testMethods.Add(TestWrapperMocking.CreateMethodDefinition("Test3", td));



            mock.Setup(_ => _.RunMsTest(It.IsAny<IEnumerable<string>>())).Returns(d);

            var ser = new MsTestService(mock.Object, new Mock<IMessageService>().Object);


            ser.LoadTests(new Collection<string> { "a" });

         
            var methods = ser.RunTests();
             
            methods.Count.ShouldEqual(2);

            methods.ElementAt(0).Name.ShouldEqual("Test1");
            methods.ElementAt(0).State.ShouldEqual(TestNodeState.Success);
            methods.ElementAt(1).State.ShouldEqual(TestNodeState.Failure);
            methods.ElementAt(1).Message.ShouldEqual("Error message");


        }


       // [Test]
        public void Wrapper()
        {
            var w = new MsTestWrapper(null);
      
            var methods = w.ReadTestMethodsFromAssembly(@"C:\Users\SysOp\Documents\Visual Studio 2010\Projects\MusicRename\MusicRename.Tests\bin\Debug\MusicRename.Tests.dll");

        }

    }
}

