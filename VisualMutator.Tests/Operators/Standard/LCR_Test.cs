namespace VisualMutator.Tests.Operators.Standard
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
    using OperatorsStandard;
    using OperatorsStandard.Operators;

    #endregion

    [TestFixture]
    public class LCR_Test
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
        public bool Method1(bool a, bool b)
        {

            bool result=false;
            bool result2=false;
            result = a && b;
            result2 = a || b;
            return result && result2;
        }
    }
}";
       //     new Conditional().;
            MutationTestsHelper.DebugTraverse(code);
            List<Mutant> mutants;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new LCR_LogicalConnectorReplacement(), out mutants, out diff);

            

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);
             //   Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }
        }
    }
}