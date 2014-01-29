namespace VisualMutator.Tests.Operators.Standard
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using NUnit.Framework;
    using OperatorsStandard;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.ExtensionMethods;

    #endregion

    [TestFixture]
    public class TestUnaryOperatorInsertion
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
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new AOR_ArithmeticOperatorReplacement(), out mutants, out original, out diff);


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
            bool c = true;
            return a;
        }
    }
}";

            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new UOI_UnaryOperatorInsertion(), out mutants, out original, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);

                codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(3);
        }
        [Test]
        public void MutationSuccessInc()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public int Method1(int a, int b)
        {
            //a++;
            return b;
        }
    }
}";
            /*
            var cci = new ModuleSource();
            cci.AppendFromFile(MutationTestsHelper.CreateModule(code));
            ModulesProvider original = new ModulesProvider(
                new ModuleSource().AppendFromFile(MutationTestsHelper.CreateModule(code)).InList());
            CodeDifferenceCreator diff;
            IList<Mutant> mutants = MutationTestsHelper.CreateMutantsLight(new UOI_UnaryOperatorInsertion(), cci, 
                100, out diff);
            var tar = mutants.Skip(1).Take(1).Single().MutationTarget;
            Console.WriteLine(tar);*/
            MutationTestsHelper.DebugTraverse(code);
            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new UOI_UnaryOperatorInsertion(), out mutants, out original, out diff);

            foreach (Mutant mutant in mutants)//.Skip(1).Take(1))
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.IL, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);

                //codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(1);
        }
    }
}