namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using NUnit.Framework;
    using OperatorsStandard;
    using Util;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;

    [TestFixture]
    public class TestRelationalOperatorReplacement
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
        public void MutationFullOnFloat()
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
            result &= a > b;
            result &= a < b;
            result &= a <= b;
            result &= a >= b;
            result &= a == b;
            result &= a != b;
            return result;
        }
    }
}";

            List<Mutant> mutants;
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new ROR_RelationalOperatorReplacement(), out mutants, out original, out diff);

            Assert.AreEqual(mutants.Count, 42);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
                Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }
        }

        [Test]
        public void MutationPartialOnObject()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(object a, object b)
        {
            bool result = true;
            result &= a == b;
            result &= a != b;
            return result;
        }
    }
}";

            List<Mutant> mutants;
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new ROR_RelationalOperatorReplacement(), out mutants, out original, out diff);


            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
                codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }
            mutants.Count(m => m.MutationTarget.PassInfo == "Equality").ShouldEqual(1);
            mutants.Count(m => m.MutationTarget.PassInfo == "NotEquality").ShouldEqual(1);
            mutants.Count(m => m.MutationTarget.PassInfo == "True").ShouldEqual(2);
            mutants.Count(m => m.MutationTarget.PassInfo == "False").ShouldEqual(2);
            mutants.Count.ShouldEqual(6);
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
        public bool Method1(int a, int b)
        {
            bool result = true;
            result &= a > b;
            result &= a < b;
            result &= a <= b;
            result &= a >= b;
            result &= a == b;
            result &= a != b;
            return result;
        }
    }
}";

            List<Mutant> mutants;
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new ROR_RelationalOperatorReplacement(), out mutants, out original, out diff);

            Assert.AreEqual(mutants.Count, 42);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
                Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }
        }
    }
}