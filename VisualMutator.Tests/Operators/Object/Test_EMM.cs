namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
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
               /* new FileAppender(){Writer = File.CreateText(@"vmlog.txt"), 
                    AppendToFile = false, Threshold = Level.Warn,
                Layout = new PatternLayout("%-5level - %date %5rms %-35.35logger{2} %-25.25method: %newline%message%newline%newline"),
                        }*/
                new ConsoleAppender
                    {
                        Layout = new PatternLayout("%-5level - %date %5rms %-35.35logger{2} %-25.25method: %newline%message%newline%newline"),
                        Threshold = Level.Warn
                    });
        }

        #endregion
        String _assemblyPath = @"D:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa\bin\Debug\Dsa.dll";
     
        [Test]
        public void Tee()
        {

            var cci = new CommonCompilerAssemblies();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);

            cci.AppendFromFile(_assemblyPath);

            var original = new AssembliesProvider(cci.Modules);
            AssembliesProvider copiedModules = new AssembliesProvider(
                cci.Modules.Select(cci.Copy).Cast<IModule>().ToList());



            var commonTargets = new List<MutationTarget>();
            var oper = new TestOperator2();
            var ded = oper.CreateVisitor();
            IOperatorCodeVisitor operatorVisitor = ded;
            operatorVisitor.Host = cci.Host;
            operatorVisitor.OperatorUtils = utils;
            operatorVisitor.Initialize();
            var mergedTargets = new List<Tuple<string /*GroupName*/, List<MutationTarget>>>();


            
            var visitor = new VisualCodeVisitor(operatorVisitor);

            var traverser = new VisualCodeTraverser(new List<TypeIdentifier>(), visitor);

            traverser.Traverse(copiedModules.Assemblies.Single());
            visitor.PostProcess();
            IEnumerable<Tuple<string, List<MutationTarget>>> s = visitor.MutationTargets.AsEnumerable();
            mergedTargets.AddRange(s);

            commonTargets.AddRange(visitor.SharedTargets);
            var mutargets = visitor.MutationTargets.Select(t => t.Item2).Flatten().ToList();

            var visitorBack = new VisualCodeVisitorBack(mutargets, new List<MutationTarget>());
            var traverser2 = new VisualCodeTraverser(new List<TypeIdentifier>(), visitorBack);
            traverser2.Traverse(copiedModules.Assemblies.Single());
            visitorBack.PostProcess();

            foreach (var pair in visitorBack.TargetAstObjects)
            {
                Assert.IsTrue(oper.visitor.objects.Any(o => int.Parse(o.Item2) == pair.Item2.CounterValue && o.Item1 == pair.Item1));
            }

            /*
          var operatorCodeRewriter = oper.CreateRewriter();
          foreach (var mutationTarget in mutargets)
          {

              var rewriter = new VisualCodeRewriter(cci.Host, visitorBack.TargetAstObjects,
                  visitorBack.SharedAstObjects, new List<TypeIdentifier>(), operatorCodeRewriter);

              operatorCodeRewriter.MutationTarget =
                  new UserMutationTarget(mutationTarget.Variant.Signature, mutationTarget.Variant.AstObjects);

              operatorCodeRewriter.NameTable = cci.Host.NameTable;
              operatorCodeRewriter.Host = cci.Host;
              operatorCodeRewriter.Module = (Module)copiedModules.Assemblies.Single();
              operatorCodeRewriter.OperatorUtils = utils;

              operatorCodeRewriter.Initialize();


              IModule rewrittenModule = rewriter.Rewrite(copiedModules.Assemblies.Single());
          }
        var rewriter = new VisualCodeRewriter(cci.Host, visitorBack.TargetAstObjects,
              visitorBack.SharedAstObjects, allowedTypes, operatorCodeRewriter);

          operatorCodeRewriter.MutationTarget =
              new UserMutationTarget(mutant.MutationTarget.Variant.Signature, mutant.MutationTarget.Variant.AstObjects);

          operatorCodeRewriter.NameTable = cci.Host.NameTable;
          operatorCodeRewriter.Host = cci.Host;
          operatorCodeRewriter.Module = (Module)copiedModules.Assemblies.Single();
          operatorCodeRewriter.OperatorUtils = utils;

          operatorCodeRewriter.Initialize();


          IModule rewrittenModule = rewriter.Rewrite(module);



          _log.Info("Got: " + mergedTargets.Select(i => i.Item2).Flatten().Count() + " mutation targets.");
         
          */

        }
        [Test]
        public void MutationAccessorSuccess()
        {
  

            List<Mutant> mutants;
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutationsFromFile(_assemblyPath, new EAM_AccessorMethodChange(), out mutants, out original, out diff);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                   original);

                if(codeWithDifference.LineChanges.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("##########################################################");
                    Console.WriteLine("EQUIVALENT MUTANT DETECTED:");
                    Console.WriteLine("From: "+mutant.MutantGroup);
                    Console.WriteLine(mutant.MutationTarget);
                    
                    Console.WriteLine(codeWithDifference.Code);

                    CodeWithDifference codeWithDifference2 = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                   original);

                }

                // codeWithDifference.LineChanges.Count.ShouldEqual(2);
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
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new EMM_ModiferMethodChange(), out mutants, out original, out diff);

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
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new EMM_ModiferMethodChange(), out mutants, out original, out diff);

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
            AssembliesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new EMM_ModiferMethodChange(), out mutants, out original, out diff);

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