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
            Console.WriteLine("Started VisualMutator.Console with params: "+ args.MakeString());

            if (args.Length >= 5)
            {
                var parser = new CommandLineParser();
                parser.ParseFrom(args);
                var connection = new EnvironmentConnection(parser);
                var boot = new ConsoleBootstrapper(connection, parser);
                boot.Initialize().Wait();
            }
            else
            {
                Console.WriteLine("Too few parameters.");
            }
        }

       
    }
}
