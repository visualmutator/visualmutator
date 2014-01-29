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
    using OperatorsObject.Operators.Other;

    #endregion

    [TestFixture]
    public class TestEqualityOperatorChange
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
        public void Equality_Is_Not_Mutated_When_Equals_Not_Overriten()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(Test test)
        {
            return new Test() == test;
        }

    }
}";

            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new EOC_EqualityOperatorChange(), out mutants, out diff);

            Assert.AreEqual(mutants.Count, 0);
        }


        [Test]
        public void Equality_Is_Not_Mutated_When_One_Type_Does_Not_Have_Equals()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class TestBase
    {
    }
    public class Test : TestBase
    {
        public bool Method1(Test test)
        {
            return new TestBase() == test;
        }
        public override bool Equals(object obj)
        {
            return true;
        }
    }
}";

            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new EOC_EqualityOperatorChange(), out mutants, out diff);

            Assert.AreEqual(mutants.Count, 1);
            Assert.AreEqual(mutants[0].MutationTarget.Variant.Signature, "Right");
            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);
                Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }
        }

        [Test]
        public void Equals_Is_Not_Mutated_When_Equals_Not_Overriten()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(Test test)
        {
            return new Test().Equals(test);
        }

    }
}";

            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new EOC_EqualityOperatorChange(), out mutants, out diff);

            Assert.AreEqual(mutants.Count, 0);
        }

        [Test]
        public void InvalidEqualityIsNotMutated()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(Test test)
        {
            return 3 == 4;
        }
        public override bool Equals(object obj)
        {
            return true;
        }
    }
}";

            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new EOC_EqualityOperatorChange(), out mutants, out diff);

            Assert.AreEqual(mutants.Count, 0);
        }

        [Test]
        public void NormalEqualityIsMutated()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(Test test)
        {
            return test == new Test();
        }
        public override bool Equals(object obj)
        {
            return true;
        }
    }
}";

            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new EOC_EqualityOperatorChange(), out mutants, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);
                Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }

            Assert.AreEqual(mutants.Count, 2);
        }


        [Test]
        public void NormalEqualsIsMutated()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(Test test)
        {
            return test.Equals(test);
        }
        public override bool Equals(object obj)
        {
            return false;
        }
    }
}";

            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new EOC_EqualityOperatorChange(), out mutants, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);
                Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }

            Assert.IsTrue(mutants.Count == 1);
        }
    }
}