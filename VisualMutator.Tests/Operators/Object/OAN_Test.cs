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
    public class OAN_Test
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
    public class Test
    {
        public void Execute()
        {
            Method1(""string1"", ""string2"", 0, 5f, 1);
        }
        public bool Method1(string s, string s2, int a, float f, int b)
        {
            return true;
        }
        public bool Method1(string s, string s2, int a, float f)
        {
            return true;
        }
    }
}";
       //     new Conditional().;
            MutationTestsHelper.DebugTraverse(code);
           
            
            List<Mutant> mutants;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new OAN_ArgumentNumberChange(), out mutants, out diff);

         

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