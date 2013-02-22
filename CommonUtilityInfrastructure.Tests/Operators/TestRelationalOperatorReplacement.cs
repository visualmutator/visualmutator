using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorTests
{
    using System.IO;
    using CommonUtilityInfrastructure;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.CodeDifference;
    using Model.Exceptions;
    using Model.Mutations;
    using Model.Mutations.Structure;
    using Model.Mutations.Types;
    using Mono.Cecil;
    using NUnit.Framework;
    using OperatorsStandard;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using Tests.Util;

    using VisualMutator.OperatorsObject.Operators;

    using Decompiler = Microsoft.Cci.ILToCodeModel.Decompiler;


    [TestFixture]
    public class TestRelationalOperatorReplacement
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
            Common.RunMutations(code, new RelationalOperatorReplacement(), out mutants, out original, out diff);

            Assert.AreEqual(mutants.Count, 42);

            foreach (var mutant in mutants)
            {
                var codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant, original);
                Console.WriteLine(codeWithDifference.Code);
                Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }

            
        }

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
            Common.RunMutations(code, new RelationalOperatorReplacement(), out mutants, out original, out diff);

            Assert.AreEqual(mutants.Count, 42);

            foreach (var mutant in mutants)
            {
                var codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant, original);
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
            Common.RunMutations(code, new RelationalOperatorReplacement(), out mutants, out original, out diff);

            

            foreach (var mutant in mutants)
            {
                var codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant, original);
                Console.WriteLine(codeWithDifference.Code);
                codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }
            mutants.Count(m=>m.MutationTarget.PassInfo == "Equality").ShouldEqual(1);
            mutants.Count(m=>m.MutationTarget.PassInfo == "NotEquality").ShouldEqual(1);
            mutants.Count(m=>m.MutationTarget.PassInfo == "True").ShouldEqual(2);
            mutants.Count(m=>m.MutationTarget.PassInfo == "False").ShouldEqual(2);
            mutants.Count.ShouldEqual(6);
        }
    }
}
