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
    using Model.StoringMutants;
    using NUnit.Framework;
    using OperatorsObject.Operators;
    using OperatorsObject.Operators.Methods;
    using SoftwareApproach.TestingExtensions;

    #endregion

    [TestFixture]
    public class OAO_Test
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

    // String _assemblyPath = @"D:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa\bin\Debug\Dsa.dll";
        [Test]
        public void MutationSuccess()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public void Execute()
        {
            Method1(""string1"", ""string2"", 0, 5f, 1);
        }
        public bool Method1(string s, string s2, int a, float f, int b)
        {
            return true;
        }
    }
}";
       //     new Conditional().;
        //    MutationTests.DebugTraverse(code);
           
           // MutationTests.DsaPath
            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new OAO_ArgumentOrderChange(), out mutants, out diff);

         

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);
             //   Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }

            mutants.Count.ShouldEqual(1);
        }
    }
}