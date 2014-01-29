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
    using OperatorsObject.Operators.Exceptions;
    using SoftwareApproach.TestingExtensions;

    #endregion

    [TestFixture]
    public class EHR_Test
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
        public int Method1(int a, int b)
        {
            int x = a;
            try
            {
                x = a - b;
            }
            
            catch(NullReferenceException e2)
            {
                x = 0;
            }
            catch(Exception e)
            {
                x = 0;
            }
            finally
            {
                x = a+b;
            }
            return x;
        }
    }
}";
            MutationTestsHelper.DebugTraverse(code);
            List<Mutant> mutants;
            IModuleSource original;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutations(code, new EHR_ExceptionHandlerRemoval(), out mutants, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant);
                Console.WriteLine(codeWithDifference.Code);

                //   codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(2);
        }
    }
}