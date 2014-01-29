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
    public class TestFieldInitializationDeletion
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
        public void T1()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        int y;
        int x = 3;
        static int z = 1;
        public bool Method1(Test test)
        {
            return true;
        }

    }
}";


           // MutationTests.DebugTraverse(code);




            var oper = new JID_FieldInitializationDeletion();
            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, oper, out mutants, out original, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
               
            }

            mutants.Count.ShouldEqual(1);
        
        }

        

    }
}