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
    public class ABS_Test
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
        public void ShouldMutateSubstraction()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public int Method1(int a, int b)
        {
            return a - b;
        }
    }
}";

            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new ABS_AbsoluteValueInsertion(), out mutants, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);

                codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(12);
        }
        [Test]
        public void ShouldMutateDecrementation()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public int Method1(int a, int b)
        {
            a--;
            return a;
        }
    }
}";

            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new ABS_AbsoluteValueInsertion(), out mutants, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.IL, mutant);
                Console.WriteLine(codeWithDifference.Code);

               // codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(12);
        }
    }
}