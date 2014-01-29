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
    using OperatorsObject.Operators.Delegates;
    using SoftwareApproach.TestingExtensions;

    #endregion

    [TestFixture]
    public class DMC_Test
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
        public delegate void Del(string s); 
        public void Method2(string s)
        {
        }
        public void Method3(string s)
        {
        }
        public bool Method1(bool a, bool b)
        {
            Del s = Method2;
            return true;
        }
    }
}";
       //     new Conditional().;
            MutationTestsHelper.DebugTraverse(code);
           
            
            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new DMC_DelegatedMethodChange(), out mutants, out diff);

            mutants.Count.ShouldEqual(1);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);
             //   Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }
        }
    }
}