namespace VisualMutator.Tests.Operators.Object
{
    #region

    using System;
    using System.Collections.Generic;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using Model.StoringMutants;
    using NUnit.Framework;
    using OperatorsObject.Operators;
    using OperatorsObject.Operators.Methods;
    using SoftwareApproach.TestingExtensions;

    #endregion

    [TestFixture]
    public class ISD_Test
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
        public void MutationSuccess()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class TestBase
    {
        public virtual void Method3(string s)
        {
            
        }
    }
    public class Test : TestBase
    {

        public override void Method3(string s)
        {
            base.Method3(s);
        }
        
    }
}";
       //     new Conditional().;
            MutationTestsHelper.DebugTraverse(code);
           
            
            List<Mutant> mutants;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new ISD_BaseKeywordDeletion(), out mutants, out diff);

            

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);
             //   Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }

            mutants.Count.ShouldEqual(1);
        }
    }
}