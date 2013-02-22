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
