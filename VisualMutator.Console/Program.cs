using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;

using CommonUtilityInfrastructure;

using Microsoft.Cci;

using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace VisualMutator.Console
{
    using Extensibility;
    using Model;
    using Model.Decompilation;
    using Model.Mutations;
    using Model.Mutations.Operators;
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

    class Program
    {
        static string _assemblyPath = @"D:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa\bin\Debug\Dsa.dll";
     
        static void Main(string[] args)
        {


            BasicConfigurator.Configure(
                /*new FileAppender(){Writer = File.CreateText(@"vmlog.txt"), 
                  AppendToFile = false, Threshold = Level.Warn,
              Layout = new PatternLayout("%-5level - %date %5rms %-35.35logger{2} %-25.25method: %newline%message%newline%newline"),
                      }*/
              new ConsoleAppender
               {
                   Layout = new PatternLayout("%-5level - %-35.35logger{2} %-25.25method: %newline%message%newline%newline"),
                   Threshold = Level.Warn
               });
            var cci = new CommonCompilerAssemblies();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);

            cci.AppendFromFile(_assemblyPath);


            var original = new AssembliesProvider(cci.Modules);

            AssembliesProvider copiedModules = new AssembliesProvider(
                cci.Modules.Select(cci.Copy).Cast<IModule>().ToList());


            AssembliesProvider copiedModules1 = new AssembliesProvider(
                cci.Modules.Select(cci.Copy).Cast<IModule>().ToList());


            AssembliesProvider copiedModules2 = new AssembliesProvider(
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

            traverser.Traverse(copiedModules2.Assemblies.Single());
            visitor.PostProcess();
            IEnumerable<Tuple<string, List<MutationTarget>>> s = visitor.MutationTargets.AsEnumerable();
            mergedTargets.AddRange(s);

            commonTargets.AddRange(visitor.SharedTargets);
            var mutargets = visitor.MutationTargets.Select(t => t.Item2).Flatten().ToList();

            var visitorBack = new VisualCodeVisitorBack(mutargets, new List<MutationTarget>());
            var traverser2 = new VisualCodeTraverser(new List<TypeIdentifier>(), visitorBack);
            traverser2.Traverse(copiedModules1.Assemblies.Single());
            visitorBack.PostProcess();
        }
    }
}
