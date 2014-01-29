namespace VisualMutator.Tests.Operators
{
    #region

    using System;
    using Extensibility;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Model;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;

    #endregion

    [TestFixture]
    public class CciAssumptions
    {

        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
       
            BasicConfigurator.Configure(
                new ConsoleAppender
                    {
                        Layout = new SimpleLayout()
                    });
        }

        #endregion

        [Test]
        public void Tree_Models_Should_Be_Identical_When_Using_Different_CCI_Instances()
        {
            var cci = new CciModuleSource();
           
            cci.AppendFromFile(MutationTestsHelper.DsaPath);
            cci.AppendFromFile(MutationTestsHelper.DsaTestsPath);

            var visitor = new DebugOperatorCodeVisitor();
            var traverser = new DebugCodeTraverser(visitor);

            traverser.Traverse(cci.Modules);

            Console.WriteLine("ORIGINAL ObjectStructure:");
            string listing0 = visitor.ToString();

            var cci2 = new CciModuleSource();
            cci2.AppendFromFile(MutationTestsHelper.DsaPath);
            cci2.AppendFromFile(MutationTestsHelper.DsaTestsPath);

            var visitor2 = new DebugOperatorCodeVisitor();
            var traverser2 = new DebugCodeTraverser(visitor2);

            traverser2.Traverse(cci2.Modules);
            string listing1 = visitor2.ToString();

            listing0.ShouldEqual(listing1);
      
        }
    }
}