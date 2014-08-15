namespace VisualMutator.Tests.Operators
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Reflection;
    using System.Security.Policy;
    using Controllers;
    using Extensibility;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.Services;
    using Model.Tests.TestsTree;
    using Ninject;
    using NUnit.Framework;
    using NUnit.Util;
    using OperatorsStandard.Operators;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using Util;
    using VisualMutator.Infrastructure;

    #endregion

    [TestFixture]
    public class MutationTestsHelper
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const string NUnitConsolePath =
            @"C:\USERS\AREGO\APPDATA\LOCAL\MICROSOFT\VISUALSTUDIO\12.0EXP\EXTENSIONS\PIOTRTRZPIL\VISUALMUTATOR\2.0.8\nunitconsole";

        public static string CompileCodeToFile(string code)
        {
            _log.Info("Parsing test code...");
            SyntaxTree tree = SyntaxTree.ParseText(code);
            _log.Info("Creating compilation...");
            Compilation comp = Compilation.Create("VisualMutator-Test-Compilation",
                new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree)
                .AddReferences(new MetadataFileReference(typeof (object).Assembly.Location));

            string outputFileName = Path.Combine(Path.GetTempPath(), "VisualMutator-Test-Compilation.lib");
            var ilStream = new FileStream(outputFileName, FileMode.OpenOrCreate);
            _log.Info("Emiting file...");
            // var pdbStream = new FileStream(Path.ChangeExtension(outputFileName, "pdb"), FileMode.OpenOrCreate);
            //  _log.Info("Emiting pdb file...");

            EmitResult result = comp.Emit(ilStream);
            ilStream.Close();
            if (!result.Success)
            {
                string aggregate = result.Diagnostics.Select(a => a.Info.GetMessage()).Aggregate((a, b) => a + "\n" + b);
                throw new InvalidProgramException(aggregate);
            }
            return outputFileName;
        }

        public static CciModuleSource CreateModuleFromCode(string code)
        {
            var path = CompileCodeToFile(code);
            return new CciModuleSource(path);
        }

        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure(
                new ConsoleAppender
                {
                    Layout = new SimpleLayout()
                });
        }

        [Test]
        public void IntegrationTestMutation()
        {

            var cci = new CciModuleSource(TestProjects.MiscUtil);

            var original = new OriginalCodebase(cci.InList());
            var type =
                cci.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Adler32") as NamedTypeDefinition;
            var method = type.Methods.First(m => m.Name.Value == "ComputeChecksum");
            var choices = new MutationSessionChoices
                          {
                              Filter = new MutationFilter(
                                  new List<TypeIdentifier>(),
                                  new MethodIdentifier(method).InList()),
                              SelectedOperators = new LOR_LogicalOperatorReplacement().InList<IMutationOperator>(),
                          };

            var exec = new MutationExecutor(new OptionsModel(), choices);
            var container = new MutantsContainer(exec, original);
            IList<AssemblyNode> assemblies = container.InitMutantsForOperators(ProgressCounter.Inactive());

            var mutants = assemblies.Cast<CheckedNode>()
                .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>())
                .OfType<Mutant>();

            var diff = new CodeDifferenceCreator();
            var vis = new CodeVisualizer(original, diff);

            foreach (var mutant in mutants)
            {
                var copy = new CciModuleSource(TestProjects.MiscUtil);
                MutationResult result = exec.ExecuteMutation(mutant, copy).Result;
                CodeWithDifference differenceListing =
                    vis.CreateDifferenceListing(CodeLanguage.CSharp, mutant, result).Result;
                differenceListing.LineChanges.Count.ShouldEqual(2);

            }


        }

    }

    public class MutMod
     {
         public Mutant Mutant { get; set; }
         public IModuleSource ModulesProvider { get; set; }

         public MutMod(Mutant mutant, IModuleSource modulesProvider)
         {
             Mutant = mutant;
             ModulesProvider = modulesProvider;
         }
     }
}