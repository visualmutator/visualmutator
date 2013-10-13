namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using NUnit.Framework;
    using OperatorsStandard;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;

    [TestFixture]
    public class TestLogicalConnectorReplacement
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
            MutationTests.DebugTraverse(code);
            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTests.RunMutations(code, new LCR_LogicalConnectorReplacement(), out mutants, out original, out diff);

            

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
             //   Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }
        }
    }
}