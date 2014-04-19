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
    using SoftwareApproach.TestingExtensions;

    #endregion

    [TestFixture]
    public class AOR_Test
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
        public void MutationFail()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(float a, float b)
        {
            bool result = true;
            string s = ""dd"";
            s = s + ""dd"";
            return result;
        }
    }
}";

            List<Mutant> mutants;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new AOR_ArithmeticOperatorReplacement(), out mutants, out diff);


            mutants.Count.ShouldEqual(0);
        }

        [Test]
        public void MutationSuccess()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public int Method1(int a, int b)
        {
            int result = 0;
            result = a + b;
            result = a - b;
            result = a * b;
            result = a / b;
            result = a % b;
            return result;
        }
    }
}";

            List<Mutant> mutants;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new AOR_ArithmeticOperatorReplacement(), out mutants, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);

                codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(30);
        }
    }
}