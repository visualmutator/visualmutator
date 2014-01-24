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
    using NUnit.Framework;
    using OperatorsObject.Operators;
    using SoftwareApproach.TestingExtensions;

    #endregion

    [TestFixture]
    public class TestOverloadingMethodContentsChange
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
        public void Test1()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
    
        public bool Method1(int x)
        {
            return x != 1;
        }

    }
}";
            var oper = new OMR_OverloadingMethodContentsChange();
            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, oper, out mutants, out original, out diff);

            Assert.AreEqual(mutants.Count, 0);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
                Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }
        }

        [Test]
        public void Test2()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
    
        public bool Method1(int x)
        {
            return x != 1;
        }
        public bool Method1(int x, int y)
        {
            return y > 1;
        }
    }
}";
            var oper = new OMR_OverloadingMethodContentsChange();
            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, oper, out mutants, out original, out diff);
            
            mutants.Count.ShouldEqual(2);
           

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
                //codeWithDifference.LineChanges.ShouldCount(2);
            }
        }
    }
}