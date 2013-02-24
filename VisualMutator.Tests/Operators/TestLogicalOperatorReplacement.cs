namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using VisualMutator.Model;
    using VisualMutator.Model.Mutations;
    using NUnit.Framework;
    using VisualMutator.OperatorsStandard;

    [TestFixture]
    public class TestLogicalOperatorReplacement
    {
        [SetUp]
        public void Setup()
        {
            log4net.Config.BasicConfigurator.Configure(
              new log4net.Appender.ConsoleAppender
              {
                  Layout = new log4net.Layout.SimpleLayout()
              });
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
            int result=0;
            result += a & b;
            result += a | b;
            result += a ^ b;
            return result;
        }
    }
}";

            List<Mutant> mutants;
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new LogicalOperatorReplacement(), out mutants, out original, out diff);

            Assert.AreEqual(mutants.Count, 12);

            foreach (var mutant in mutants)
            {
                var codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant, original);
                Console.WriteLine(codeWithDifference.Code);
                Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }

            
        }



    }
}
