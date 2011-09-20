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
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
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

            
            
            var td = new TypeDefinition("ns2", "Class1", TypeAttributes.Public);

            td.Methods.Add(CecilUtils.CreateMethodDefinition("Test1", td));
            td.Methods.Add(CecilUtils.CreateMethodDefinition("Test2", td));
            td.Methods.Add(CecilUtils.CreateMethodDefinition("Test3", td));
          //  AssemblyDefinition assembly = TestWrapperMocking.CreateAssembly("Ass", new[] { td });

            //  var mock = TestWrapperMocking.MockMsTestWrapperForLoad(out testMethods);

            var msTestLoaderMock = new Mock<IMsTestLoader>();
            msTestLoaderMock.Setup(_ => _.ScanAssemblies(It.IsAny<IEnumerable<string>>())).Returns(new AssemblyScanResult
            {
                AssembliesWithTests = new[] { "Ass"},
                TestMethods = td.Methods
            });
            var ser = new MsTestService(null, msTestLoaderMock.Object);


            IEnumerable<TestNodeClass> testNodeClasses = ser.LoadTests(new Collection<string> { "a"});


            var testFixture = testNodeClasses.Single();

            testFixture.Children.Count.ShouldEqual(3);
            testFixture.Children.ElementAt(1).Name.ShouldEqual("Test2");
            testFixture.Children.ElementAt(2).Name.ShouldEqual("Test3");
        }
        [Test]
        public void RunTests_Returns_Empty_List_When_No_Assemblies_With_Tests()
        {
            var msTestLoaderMock = new Mock<IMsTestLoader>();
            msTestLoaderMock.Setup(_ => _.ScanAssemblies(It.IsAny<IEnumerable<string>>()))
            .Returns(new AssemblyScanResult
            {
                AssembliesWithTests = new string[0],
                TestMethods = new List<MethodDefinition>(),
            });

     
            var ser = new MsTestService(null, msTestLoaderMock.Object);


            ser.LoadTests(new Collection<string> { "any" });

            var methods = ser.RunTests();

            methods.Count.ShouldEqual(0);


        }

        [Test]
        public void RunningTestsNormally()
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



            var td = new TypeDefinition("ns2", "Class1", TypeAttributes.Public);

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
            
            var mock = new Mock<IMsTestWrapper>();
            mock.Setup(_ => _.RunMsTest(It.IsAny<IEnumerable<string>>())).Returns(d);
            var ser = new MsTestService(mock.Object, msTestLoaderMock.Object);

           
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
            var w = new MsTestLoader(new AssemblyReaderWriter());
      
            var methods = w.ReadTestMethodsFromAssembly(@"C:\Users\SysOp\Documents\Visual Studio 2010\Projects\MusicRename\MusicRename.Tests\bin\Debug\MusicRename.Tests.dll");

        }

    }
}

