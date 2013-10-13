namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Paths;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Mutations;
    using NUnit.Framework;
    using OperatorsObject.Operators;
    using Util;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Core;
    using log4net.Layout;

    [TestFixture]
    public class Test_EMM
    {
        public class TestOperator2 : IMutationOperator
        {
            public class OperatorCodeVisitor2 : OperatorCodeVisitor
            {
                public List<Tuple<object, string>> objects = new List<Tuple<object, string>>(); 
                private int count;
                public override void VisitAny(object o)
                {
                    count++;
                    objects.Add(Tuple.Create(o, count.ToString()));
                    MarkMutationTarget(o, count.ToString());
                    
                }
            }
            public class OperatorCodeRewriter2 : OperatorCodeRewriter
            {
                private readonly OperatorCodeVisitor2 _operatorCodeVisitor2;

                public OperatorCodeRewriter2(OperatorCodeVisitor2 operatorCodeVisitor2)
                {
                    _operatorCodeVisitor2 = operatorCodeVisitor2;
            
                }

                public override IExpression Rewrite(IExpression expre)
                {

                    return expre;
                }
            }
            #region IMutationOperator Members

            public OperatorInfo Info
            {
                get
                {
                    return new OperatorInfo("Test", "", "");
                }
            }

            public OperatorCodeVisitor2 visitor;
            public OperatorCodeRewriter2 rewriter2;
            public TestOperator2()
            {
                visitor = new OperatorCodeVisitor2();
                rewriter2 = new OperatorCodeRewriter2(visitor);
            }

            public IOperatorCodeVisitor CreateVisitor()
            {
                return visitor;
            }

            public IOperatorCodeRewriter CreateRewriter()
            {
                return rewriter2;
            }

            #endregion
        }

        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure(
                /*new FileAppender(){Writer = File.CreateText(@"vmlog.txt"), 
                   AppendToFile = false, Threshold = Level.Warn,
               Layout = new PatternLayout("%-5level - %date %5rms %-35.35logger{2} %-25.25method: %newline%message%newline%newline"),
                       }*/
            new ConsoleAppender
            {
                Layout = new PatternLayout("%-5level - %date %5rms %-35.35logger{2} %-25.25method: %newline%message%newline%newline"),
                Threshold = Level.Debug
            });
         
        }

        #endregion

      
        [Test]
        public void Tee()
        {

        
            var cci = new CommonCompilerInfra();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);

            cci.AppendFromFile(MutationTests.DsaPath);

            var original = new ModulesProvider(cci.Modules);
            ModulesProvider copiedModules = new ModulesProvider(
                cci.Modules.Select(cci.Copy).Cast<IModule>().ToList());



            var commonTargets = new List<MutationTarget>();
            var oper = new TestOperator2();
            var ded = oper.CreateVisitor();
            IOperatorCodeVisitor operatorVisitor = ded;
            operatorVisitor.Host = cci.Host;
            operatorVisitor.OperatorUtils = utils;
            operatorVisitor.Initialize();
            var mergedTargets = new List<MutationTarget>();



            var visitor = new VisualCodeVisitor(operatorVisitor, copiedModules.Assemblies.Single());

            var traverser = new VisualCodeTraverser(new List<TypeIdentifier>(), visitor);

            traverser.Traverse(copiedModules.Assemblies.Single());
            visitor.PostProcess();
         //   IEnumerable<Tuple<string, List<MutationTarget>>> s = visitor.MutationTargets.AsEnumerable();
            mergedTargets.AddRange(visitor.MutationTargets);

            commonTargets.AddRange(visitor.SharedTargets);

            var visitorBack = new VisualCodeVisitorBack(visitor.MutationTargets, new List<MutationTarget>(), 
                copiedModules.Assemblies.Single());
            var traverser2 = new VisualCodeTraverser(new List<TypeIdentifier>(), visitorBack);
            traverser2.Traverse(copiedModules.Assemblies.Single());
            visitorBack.PostProcess();
            /*
            foreach (var pair in visitorBack.TargetAstObjects)
            {
                Assert.IsTrue(oper.visitor.objects.Any(o => int.Parse(o.Item2) == pair.Item2.CounterValue && o.Item1 == pair.Item1));
            }
            */
          
        }
        [Test]
        public void MutationAccessorSuccess()
        {
  

            
        }
        [Test]
        public void Mutation_Of_Two_Modules()
        {
            var oper = new EAM_AccessorMethodChange();
            ///////
            var cci = new CommonCompilerInfra();
            var utils = new OperatorUtils(cci);
            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);
            List<AssemblyNode> assemblyNodes = new List<AssemblyNode>
            {
                new AssemblyNode("", cci.AppendFromFile(MutationTests.DsaPath))
                {
                    AssemblyPath = new FilePathAbsolute(MutationTests.DsaPath)
                },
                new AssemblyNode("", cci.AppendFromFile(MutationTests.DsaTestsPath))
                {
                    AssemblyPath = new FilePathAbsolute(MutationTests.DsaTestsPath)
                }
            };
            var original = new ModulesProvider(cci.Modules);
            cache.setDisabled(disableCache: true);
            var diff = new CodeDifferenceCreator(cache, visualizer);
            container.DebugConfig = true;
            var mutmods = MutationTests.CreateMutants(oper, container, assemblyNodes, cache, 1);
            var mutants = mutmods.Select(m => m.Mutant).ToList();

            foreach (Mutant mutant in mutants.Take(1))
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                   original);
                Console.WriteLine(codeWithDifference.Code);
              
            }

            mutants.Count.ShouldEqual(1);
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
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTests.RunMutations(code, new EMM_ModiferMethodChange(), out mutants, out original, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);

                // codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(1);
        }
        [Test]
        public void Fail()
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
        public int TestProp2 {  get{return 0;} }

    }
}";

            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTests.RunMutations(code, new EMM_ModiferMethodChange(), out mutants, out original, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);

                // codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(0);
        }

        [Test]
        public void Fail2()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test<T>
    {
        public void Method1(int a, int b)
        {
            TestProp = null;
        }

        public Test<T> TestProp {  get;set; }
        public Test<T> Previous {  get;set; }

    }
}";

            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTests.RunMutations(code, new EMM_ModiferMethodChange(), out mutants, out original, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);

                // codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }

            mutants.Count.ShouldEqual(1);
        }
    }
}