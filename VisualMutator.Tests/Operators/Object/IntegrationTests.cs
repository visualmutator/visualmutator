namespace VisualMutator.Tests.Operators.Object
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensibility;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Microsoft.Cci;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using NUnit.Framework;
    using OperatorsObject.Operators;

    #endregion

    [TestFixture]
    public class IntegrationTests
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

        
        public void RunMutations(IMutationOperator oper, String path)
        {
            var cci = new CciModuleSource();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);

            cci.AppendFromFile(path);

            var original = new IModuleSource(cci.Modules);
            IModuleSource copiedModules = new IModuleSource(
                cci.Modules.Select(cci.Copy).Cast<IModule>().ToList());

            var commonTargets = new List<MutationTarget>();
            
            var ded = oper.CreateVisitor();
            IOperatorCodeVisitor operatorVisitor = ded;
            operatorVisitor.Host = cci.Host;
            operatorVisitor.OperatorUtils = utils;
            operatorVisitor.Initialize();
            var mergedTargets = new List<MutationTarget>();



            var visitor = new VisualCodeVisitor("", operatorVisitor, copiedModules.Modules.Single());

            var traverser = new VisualCodeTraverser(MutationFilter.AllowAll(), visitor);

            traverser.Traverse(copiedModules.Modules.Single());
            visitor.PostProcess();
            //   IEnumerable<Tuple<string, List<MutationTarget>>> s = visitor.MutationTargets.AsEnumerable();
            mergedTargets.AddRange(visitor.MutationTargets);

            commonTargets.AddRange(visitor.SharedTargets);

            var visitorBack = new VisualCodeVisitorBack(visitor.MutationTargets, new List<MutationTarget>(),
                copiedModules.Modules.Single(), "");
            var traverser2 = new VisualCodeTraverser(MutationFilter.AllowAll(), visitorBack);
            traverser2.Traverse(copiedModules.Modules.Single());
            visitorBack.PostProcess();
            /*
            foreach (var pair in visitorBack.TargetAstObjects)
            {
                Assert.IsTrue(oper.visitor.objects.Any(o => int.Parse(o.Item2) == pair.Item2.CounterValue && o.Item1 == pair.Item1));
            }
            */

        }

        private string modulePath = MutationTestsHelper.DsaPath;


        [Test]
        public void Test_DEH_MethodDelegatedForEventHandlingChange()
        {
            var oper = new DEH_MethodDelegatedForEventHandlingChange();
            List<Mutant> mutants;
            IModuleSource originalModules;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutationsFromFile(modulePath, oper, 
                out mutants, out originalModules, out diff);
            Assert.Pass();
        }
        [Test]
        public void Test_DMC_DelegatedMethodChange()
        {
            var oper = new DMC_DelegatedMethodChange();
            List<Mutant> mutants;
            IModuleSource originalModules;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutationsFromFile(modulePath, oper,
                out mutants, out originalModules, out diff);
            Assert.Pass();
        }
        [Test]
        public void Test_EHC_ExceptionHandlingChange()
        {
            var oper = new EHC_ExceptionHandlingChange();
            List<Mutant> mutants;
            IModuleSource originalModules;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutationsFromFile(modulePath, oper,
                out mutants, out originalModules, out diff);
            Assert.Pass();
        }
        [Test]
        public void Test_EHR_ExceptionHandlerRemoval()
        {
            var oper = new EHR_ExceptionHandlerRemoval();
            List<Mutant> mutants;
            IModuleSource originalModules;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutationsFromFile(modulePath, oper,
                out mutants, out originalModules, out diff);
            Assert.Pass();
        }
        [Test]
        public void Test_EXS_ExceptionSwallowing()
        {
            var oper = new EXS_ExceptionSwallowing();
            List<Mutant> mutants;
            IModuleSource originalModules;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutationsFromFile(modulePath, oper,
                out mutants, out originalModules, out diff);
            Assert.Pass();
        }
        [Test]
        public void Test_ISD_BaseKeywordDeletion()
        {
            var oper = new ISD_BaseKeywordDeletion();
            List<Mutant> mutants;
            IModuleSource originalModules;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutationsFromFile(modulePath, oper,
                out mutants, out originalModules, out diff);
            Assert.Pass();
        }
        [Test]
        public void Test_MCI_MemberCallFromAnotherInheritedClass()
        {
            var oper = new MCI_MemberCallFromAnotherInheritedClass();
            List<Mutant> mutants;
            IModuleSource originalModules;
            CodeDifferenceCreator diff;
            MutationTestsHelper.RunMutationsFromFile(modulePath, oper,
                out mutants, out originalModules, out diff);
            Assert.Pass();
        }
    }
}