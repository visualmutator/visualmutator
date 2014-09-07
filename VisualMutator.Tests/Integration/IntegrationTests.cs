namespace VisualMutator.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Reflection;
    using System.Threading.Tasks;
    using Controllers;
    using Extensibility;
    using log4net;
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
    using NUnit.Core;
    using NUnit.Framework;
    using NUnit.Util;
    using Operators;
    using OperatorsStandard.Operators;
    using Roslyn.Compilers.CSharp;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using Util;
    using Views;
    using VisualMutator.Infrastructure;

    [TestFixture]
    public class IntegrationTests : IntegrationTest
    {
        [Test]
        public void IntegrationTestingMiscUtilLight()
        {
            var cci = new CciModuleSource(TestProjects.MiscUtil);
            var choices = new MutationSessionChoices();
            var tt = cci.Module.Module.GetAllTypes();
          //  var type = (NamedTypeDefinition)cci.Module.Module.GetAllTypes().Single(t => t.Name.Value == "Range");
           // var method = new MethodIdentifier(type.Methods.First(m => m.Name.Value == "Contains"));
            MethodIdentifier method = null;
            choices.SelectedOperators.Add(new IdentityOperator2());
            choices.Filter = method == null ? MutationFilter.AllowAll() :
                                 new MutationFilter(
                                  new List<TypeIdentifier>(),
                                  method.InList());

            var options = new OptionsModel();
            var muexe = new MutationExecutor(options, choices);
            var mucon = new MutantsContainer(muexe, new OriginalCodebase(cci.InList()));
            var nodes = mucon.InitMutantsForOperators(ProgressCounter.Inactive());
            Mutant mutant = nodes.Cast<CheckedNode>()
              .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>())
              .OfType<Mutant>().First();//.Take(4);
            var cciWhite = new CciModuleSource(TestProjects.MiscUtil);
            var resu = muexe.ExecuteMutation(mutant, cciWhite).Result;
            string filePath = @"C:\PLIKI\VisualMutator\testprojects\MiscUtil-Integration-Tests\TestGround\MiscUtil.dll";
            string filePathTests = @"C:\PLIKI\VisualMutator\testprojects\MiscUtil-Integration-Tests\TestGround\MiscUtil.UnitTests.dll";
            using (var file = File.OpenWrite(filePath))
            {
                resu.MutatedModules.WriteToStream(resu.MutatedModules.Modules.Single(), file, filePath);
            }

            var runContext = new NUnitTestsRunContext(
                options,
                _kernel.Get<IProcesses>(),
                _kernel.Get<CommonServices>(),
                new NUnitResultsParser(),
                TestProjects.NUnitConsolePath,
                filePathTests,
                new TestsSelector());
            var testResult = runContext.RunTests().Result;
            
            var count = testResult.ResultMethods
                .GroupBy(t => t.State)
                .ToDictionary(t => t.Key);
            var countStrings = count.Select(pair => pair.Key.ToString() + ": " + pair.Value.Count());
            _log.Info(string.Format("All test results: " + string.Join(" ", countStrings)));
            count[TestNodeState.Failure].Count().ShouldEqual(0);
           
        }

        [Test]
        public void IntegrationTestingMiscUtil()
        {
            var paths = new[] {
                 TestProjects.MiscUtil,
                 TestProjects.MiscUtilTests}.Select(_ => _.ToFilePathAbs()).ToList();

            var cci = new CciModuleSource(TestProjects.MiscUtil);
            var cciTests = new CciModuleSource(TestProjects.MiscUtilTests);

            var toMutate = TestProjects.MiscUtil.InList();
            var original = new OriginalCodebase(new List<CciModuleSource> {cci, cciTests});

            var type = (NamedTypeDefinition)cci.Module.Module.GetAllTypes().Single(t => t.Name.Value == "Range");
            var method = type.Methods.First(m => m.Name.Value == "Contains");

            var oper = new ROR_RelationalOperatorReplacement().InList<IMutationOperator>();

            var mutants = SetupMutations(original, paths, toMutate, oper, new MethodIdentifier(method));

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
                    .SelectManyRecursive(n => n.Children, leafsOnly: true).OfType<TestNodeMethod>();

             //   vis.CreateDifferenceListing()
                meth.Count(m => m.State == TestNodeState.Failure).ShouldBeGreaterThan(0);
                //  var storedMutantInfo = muma.StoreMutant(mutant).Result;

                //  RunTestsForMutant(_choices.MutantsTestingOptions, _storedMutantInfo);

                //   CodeWithDifference differenceListing = vis.CreateDifferenceListing(CodeLanguage.CSharp, mutant, result).Result;
                //     differenceListing.LineChanges.Count.ShouldEqual(2);

            }

        }

        [Test]
        public void ReadWriteTestAutoMapper()
        {
            var cci = new CciModuleSource(TestProjects.AutoMapper);
            using (var file = File.OpenWrite(@"C:\PLIKI\VisualMutator\testprojects\Automapper-Integration-Tests\readwrite\AutoMapper.dll"))
            {
                cci.WriteToStream(cci.Module, file, file.Name);
            }
            
        }

        [Test]
        public void IntegrationTestingAutoMapper()
        {
            var paths = new[] {
                 TestProjects.AutoMapper,
                 TestProjects.AutoMapperNet4,
                 TestProjects.AutoMapperTests
            }.Select(_ => _.ToFilePathAbs()).ToList();

            var cci = new CciModuleSource(TestProjects.AutoMapper);
            var cci1 = new CciModuleSource(TestProjects.AutoMapperNet4);
            var cciTests = new CciModuleSource(TestProjects.AutoMapperTests);

            var original = new OriginalCodebase(new List<CciModuleSource> {cci, cci1, cciTests});
            var toMutate = new List<string>{
                    TestProjects.AutoMapper,
                 TestProjects.AutoMapperNet4};

            var oper = new IdentityOperator2().InList<IMutationOperator>();

            var mutants = SetupMutations(original, paths, toMutate, oper);

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
                    .SelectManyRecursive(n => n.Children, leafsOnly: true).OfType<TestNodeMethod>();

                meth.Count(m => m.State == TestNodeState.Failure).ShouldEqual(0);
                //  var storedMutantInfo = muma.StoreMutant(mutant).Result;

                //  RunTestsForMutant(_choices.MutantsTestingOptions, _storedMutantInfo);

                //   CodeWithDifference differenceListing = vis.CreateDifferenceListing(CodeLanguage.CSharp, mutant, result).Result;
                //     differenceListing.LineChanges.Count.ShouldEqual(2);

            }

        }


        private IEnumerable<Mutant> SetupMutations(OriginalCodebase original, 
            List<FilePathAbsolute> paths, List<string> toMutate, 
            List<IMutationOperator> operators, MethodIdentifier method= null)
        {

            var options = new OptionsModel();
            options.OtherParams = "--debugfiles true";

            _kernel.Bind<IProjectClonesManager>().To<ProjectClonesManager>().InSingletonScope();
            _kernel.Bind<ProjectFilesClone>().ToSelf().AndFromFactory();
            _kernel.Bind<FilesManager>().ToSelf().InSingletonScope();
            _kernel.Bind<TestServiceManager>().ToSelf().InSingletonScope();
            _kernel.Bind<XUnitTestService>().ToSelf().InSingletonScope();
            _kernel.Bind<XUnitResultsParser>().ToSelf().InSingletonScope();
            _kernel.Bind<WhiteCache>().ToSelf().AndFromFactory();

            _kernel.Bind<INUnitWrapper>().To<NUnitWrapper>().InSingletonScope();
            _kernel.Bind<OriginalCodebase>().ToConstant(original);
            _kernel.Bind<ICodeDifferenceCreator>().To<CodeDifferenceCreator>().InSingletonScope();
            _kernel.Bind<ICodeVisualizer>().To<CodeVisualizer>().InSingletonScope();
            _kernel.Bind<IMutantsCache>().To<MutantsCache>().InSingletonScope();
            _kernel.Bind<NUnitTestsRunContext>().ToSelf().AndFromFactory();
            _kernel.Bind<XUnitTestsRunContext>().ToSelf().AndFromFactory();
            _kernel.Bind<OptionsModel>().ToConstant(options);
            _kernel.Bind<IMutationExecutor>().To<MutationExecutor>().InSingletonScope();
            _kernel.Bind<TestingMutant>().ToSelf().AndFromFactory();
            _kernel.Bind<TestLoader>().ToSelf().AndFromFactory();

            _kernel.BindMock<IHostEnviromentConnection>(mock =>
            {
                mock.Setup(_ => _.GetProjectAssemblyPaths()).Returns(paths);
                mock.Setup(_ => _.GetTempPath()).Returns(Path.GetTempPath());
            });

            _kernel.Get<ISettingsManager>()["NUnitConsoleDirPath"] = TestProjects.NUnitConsoleDirPath;
            _kernel.Get<ISettingsManager>()["XUnitConsoleDirPath"] = TestProjects.XUnitConsoleDirPath;

            _kernel.Bind<IWhiteSource>().ToConstant(new WhiteDummy(toMutate));

            var testsClone = _kernel.Get<IProjectClonesManager>().CreateClone("Tests");
            var testsTask = _kernel.Get<TestsLoader>().LoadTests(testsClone.Assemblies.AsStrings().ToList());

            var strategy = new AllTestsSelectStrategy(testsTask);
         
            var choices = new MutationSessionChoices
                          {
                              Filter = method == null ? MutationFilter.AllowAll() :
                                 new MutationFilter(
                                  new List<TypeIdentifier>(),
                                  method.InList()),
                              SelectedOperators = operators,
                              TestAssemblies = strategy.SelectTests().Result
                          };
            _kernel.Bind<MutationSessionChoices>().ToConstant(choices);

            var exec = _kernel.Get<MutationExecutor>();
            var container = new MutantsContainer(exec, original);
            IList<AssemblyNode> assemblies = container.InitMutantsForOperators(ProgressCounter.Inactive());

            var mutants = assemblies.Cast<CheckedNode>()
                .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>())
                .OfType<Mutant>();//.Take(4);
            return mutants;
        }
    }
}