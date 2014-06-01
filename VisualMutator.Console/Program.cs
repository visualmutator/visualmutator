using System;
using System.Collections.Generic;
using System.IO;


using Microsoft.Cci;

using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace VisualMutator.Console
{
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Extensibility;
    using Infrastructure;
    using Model;
    using Model.Decompilation;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.Services;
    using Model.Tests.TestsTree;
    using Model.Verification;
    using Moq;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;
    using Console = System.Console;

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
        public Program()
        {
            T2();
            T1();
        }



     //   private static ICollection<TestId> _selectedTests;

        private async Task<object> L1()
        {
            int i = 0;
            return await Task.Run(() =>
            {
                while (i >= 0)
                {
                }
                return new object();
            });
        }
        private async void T2()
        {
            await L1();
        }


        private async void T1()
        {
            await L1();
        }
        private static void Main(string[] args)
        {
            if (args.Length >= 3)
            {
                var parser = new CommandLineParser(args);
                var connection = new EnvironmentConnection(parser);
                var boot = new ConsoleBootstrapper(connection, parser);
                boot.Initialize();
            }
            else
            {
                Console.WriteLine("Too few parameters.");
            }
        }

       
    }
}
