namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommonUtilityInfrastructure.Paths;
    using Extensibility;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using NUnit.Framework;
    using OperatorsStandard;
    using Util;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;

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
            var cci = new CommonCompilerInfra();
           
            cci.AppendFromFile(MutationTests.DsaPath);
            cci.AppendFromFile(MutationTests.DsaTestsPath);

            var visitor = new DebugOperatorCodeVisitor();
            var traverser = new DebugCodeTraverser(visitor);

            traverser.Traverse(cci.Modules);

            Console.WriteLine("ORIGINAL ObjectStructure:");
            string listing0 = visitor.ToString();

            var cci2 = new CommonCompilerInfra();
            cci2.AppendFromFile(MutationTests.DsaPath);
            cci2.AppendFromFile(MutationTests.DsaTestsPath);

            var visitor2 = new DebugOperatorCodeVisitor();
            var traverser2 = new DebugCodeTraverser(visitor2);

            traverser2.Traverse(cci2.Modules);
            string listing1 = visitor2.ToString();

            listing0.ShouldEqual(listing1);
      
        }
    }
}