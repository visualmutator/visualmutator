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
    using System.Threading.Tasks;
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
            var type = cci.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Adler32") as NamedTypeDefinition;
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
                CodeWithDifference differenceListing = vis.CreateDifferenceListing(CodeLanguage.CSharp, mutant, result).Result;
                differenceListing.LineChanges.Count.ShouldEqual(2);

            }


        }


        class WhiteDummy : IWhiteSource
        {
            private readonly List<string> _paths;

            public WhiteDummy(List<string> paths)
            {
                _paths = paths;
            }

            public Task Initialize()
            {
                return Task.FromResult(0);
            }

            public Task<List<CciModuleSource>> GetWhiteModulesAsync()
            {
                return Task.FromResult(_paths.Select(p => new CciModuleSource(p)).ToList());
                
            }

            public void Dispose()
            {
            }

            public void Pause(bool paused)
            {
            }

            public Task<CciModuleSource> GetWhiteSourceAsync(string moduleName)
            {
                var path =_paths.Single(p => Path.GetFileNameWithoutExtension(p) == moduleName);
                return Task.FromResult(new CciModuleSource(path));
            }

            public void ReturnToCache(string name, CciModuleSource whiteModules)
            {
            }

            public Task<List<CciModuleSource>> GetWhiteModulesAsyncOld()
            {
                return null;
            }
        }

        class MapSettingsManager : ISettingsManager
        {
            private Dictionary<string, string> _settings; 
            public MapSettingsManager()
            {
                _settings = new Dictionary<string, string>();
            }

            public void Initialize()
            {
                
            }

            public bool ContainsKey(string key)
            {
                return _settings.ContainsKey(key);
            }

            public string this[string key]
            {
                get { return _settings[key]; }
                set
                {
                    _settings[key] = value;
                }
            }
        }

        [Test]
        public void IntegrationTesting()
        {
            var paths = new[] {
                 TestProjects.MiscUtil,
                 TestProjects.MiscUtilTests}.Select(_ => _.ToFilePathAbs()).ToList();

            var cci = new CciModuleSource(TestProjects.MiscUtil);
            var cci2 = new CciModuleSource(TestProjects.MiscUtilTests);

            var original = new OriginalCodebase(new List<CciModuleSource> { cci, cci2 });

            var _kernel = new StandardKernel();
            _kernel.Load(new IntegrationTestModule());

            _kernel.Bind<IProjectClonesManager>().To<ProjectClonesManager>().InSingletonScope();
            _kernel.Bind<ProjectFilesClone>().ToSelf().AndFromFactory();
            _kernel.Bind<FilesManager>().ToSelf().InSingletonScope();
            _kernel.Bind<WhiteCache>().ToSelf().AndFromFactory();

            _kernel.Bind<ISettingsManager>().To<MapSettingsManager>().InSingletonScope();
            _kernel.Bind<INUnitWrapper>().To<NUnitWrapper>().InSingletonScope();
            _kernel.Bind<OriginalCodebase>().ToConstant(original);
            _kernel.Bind<ICodeDifferenceCreator>().To<CodeDifferenceCreator>().InSingletonScope();
            _kernel.Bind<ICodeVisualizer>().To<CodeVisualizer>().InSingletonScope();
            _kernel.Bind<IMutantsCache>().To<MutantsCache>().InSingletonScope();
            _kernel.Bind<TestsRunContext>().ToSelf().AndFromFactory();
            _kernel.Bind<OptionsModel>().ToConstant(new OptionsModel());
            _kernel.Bind<IMutationExecutor>().To<MutationExecutor>().InSingletonScope();
            _kernel.Bind<TestingMutant>().ToSelf().AndFromFactory();
            _kernel.Bind<TestLoader>().ToSelf().AndFromFactory();

            _kernel.BindMock<IHostEnviromentConnection>(mock =>
            {
                mock.Setup(_ => _.GetProjectAssemblyPaths()).Returns(paths);
                mock.Setup(_ => _.GetTempPath()).Returns(Path.GetTempPath());
            });

            _kernel.Get<ISettingsManager>()["NUnitConsoleDirPath"] = NUnitConsolePath;

            _kernel.Bind<IWhiteSource>().ToConstant(new WhiteDummy(TestProjects.MiscUtil.InList()));

            var testsClone = _kernel.Get<IProjectClonesManager>().CreateClone("Tests");
            var s = _kernel.Get<TestsLoader>().LoadTests(testsClone.Assemblies.AsStrings().ToList()).CastTo<object>();

            var strategy = new AllTestsSelectStrategy(Task.FromResult(s));
            var type = cci.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Adler32") as NamedTypeDefinition;
            var method = type.Methods.First(m => m.Name.Value == "ComputeChecksum");
            var choices = new MutationSessionChoices
            {
                Filter = new MutationFilter(
                                  new List<TypeIdentifier>(),
                                  new MethodIdentifier(method).InList()),
                SelectedOperators = new LOR_LogicalOperatorReplacement().InList<IMutationOperator>(),
                TestAssemblies = strategy.SelectTests().Result
            };
            _kernel.Bind<MutationSessionChoices>().ToConstant(choices);

            var exec = _kernel.Get<MutationExecutor>();
            var container = new MutantsContainer(exec, original);
            IList<AssemblyNode> assemblies = container.InitMutantsForOperators(ProgressCounter.Inactive());

            var mutants = assemblies.Cast<CheckedNode>()
                .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>())
                .OfType<Mutant>().Take(2);

            var vis = _kernel.Get<ICodeVisualizer>();


            var muma = _kernel.Get<MutantMaterializer>();


            IObserver<SessionEventArgs> sub = new ReplaySubject<SessionEventArgs>();
            foreach (var mutant in mutants)
            {
              //  var copy = new CciModuleSource(TestProjects.MiscUtil);
            //    MutationResult result = exec.ExecuteMutation(mutant, copy).Result;
                var muma2 = _kernel.Get<IFactory<TestingMutant>>().CreateWithParams(sub, mutant);


                var r = muma2.RunAsync().Result;
                var namespaces = _kernel.Get<TestsContainer>().CreateMutantTestTree(mutant);
                var meth = namespaces.Cast<CheckedNode>()
                    .SelectManyRecursive(n => n.Children, leafsOnly:true).OfType<TestNodeMethod>();

                meth.Count(m => m.State == TestNodeState.Failure).ShouldEqual(55);
                //  var storedMutantInfo = muma.StoreMutant(mutant).Result;

                //  RunTestsForMutant(_choices.MutantsTestingOptions, _storedMutantInfo);

                //   CodeWithDifference differenceListing = vis.CreateDifferenceListing(CodeLanguage.CSharp, mutant, result).Result;
                //     differenceListing.LineChanges.Count.ShouldEqual(2);

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