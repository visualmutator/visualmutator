namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CSharpSourceEmitter;
    using Microsoft.Cci;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using VisualMutator.Model;
    using VisualMutator.Model.Mutations;
    using NUnit.Framework;
    using VisualMutator.OperatorsStandard;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class TestAbsoluteValueInsertion
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
            return a - b;
        }
    }
}";

            List<Mutant> mutants;
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new AbsoluteValueInsertion(), out mutants, out original, out diff);

            foreach (var mutant in mutants)
            {
            
                var codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant, original);
                Console.WriteLine(codeWithDifference.Code);

                codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(3);
        }
        
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
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new ArithmeticOperatorReplacement(), out mutants, out original, out diff);

        

            mutants.Count.ShouldEqual(0);
        }
        
    }
}
