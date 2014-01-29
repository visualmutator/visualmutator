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
    using OperatorsObject.Operators.Variables;
    using SoftwareApproach.TestingExtensions;

    #endregion

    [TestFixture]
    public class MCI_Test
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
        Test a = new Test();
        Test b = new Test();
        public void Method1(bool b1, bool b2)
        {
           a.Method1(true, true);
        }
    }
}";
       //     new Conditional().;
            MutationTestsHelper.DebugTraverse(code);
           
            
            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new MCI_MemberCallFromAnotherInheritedClass(), out mutants, out diff);

         

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