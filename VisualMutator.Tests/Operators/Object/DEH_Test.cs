namespace VisualMutator.Tests.Operators
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
    public class DEH_Test
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
        public delegate void Del(string s); 
        public event Del Event1;
        public void Method2(string s)
        {
        }
        public void Method3(string s)
        {
            Event1 -= Method3;
        }
        public bool Method1(bool a, bool b)
        {
            Event1 += Method2;
            return true;
        }
    }
}";
       //     new Conditional().;
            MutationTests.DebugTraverse(code);
           
            
            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTests.RunMutations(code, new DEH_MethodDelegatedForEventHandlingChange(), out mutants, out original, out diff);

         

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
             //   Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }

            mutants.Count.ShouldEqual(2);
        }
    }
}