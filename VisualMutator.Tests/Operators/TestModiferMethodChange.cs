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
    using OperatorsObject.Operators;
    using VisualMutator.Model;
    using VisualMutator.Model.Mutations;
    using NUnit.Framework;
    using VisualMutator.OperatorsStandard;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class TestModiferMethodChange
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
        public void MutationModifierSuccess()
        {

            const string code =
@"using System;
namespace Ns
{
    public class Test
    {
        public int Method1(int a, int b)
        {
            TestProp = b;
            return TestProp;
        }

        public int TestProp {  get;set; }
        public int TestProp2 {  get;set; }

    }
}";

            List<Mutant> mutants;
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new ModiferMethodChange(), out mutants, out original, out diff);

            foreach (var mutant in mutants)
            {
                var codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant, original);
                Console.WriteLine(codeWithDifference.Code);

               // codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(1);
        }

        [Test]
        public void MutationAccessorSuccess()
        {

            const string code =
@"using System;
namespace Ns
{
    public class Test
    {
        public int Method1(int a, int b)
        {
            return TestProp;
        }

        public int TestProp {  get;set; }
        public int TestProp2 {  get;set; }

    }
}";

            List<Mutant> mutants;
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new AccessorMethodChange(), out mutants, out original, out diff);

            foreach (var mutant in mutants)
            {
                var codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant, original);
                Console.WriteLine(codeWithDifference.Code);

                // codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(1);
        }
       
        
    }
}
