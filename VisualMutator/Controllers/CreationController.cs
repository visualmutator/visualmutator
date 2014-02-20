namespace VisualMutator.Controllers
{
    #region

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Infrastructure;
    using log4net;
    using Model;
    using Model.Exceptions;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.TestsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using UsefulTools.Wpf;
    using ViewModels;

    #endregion

    public class CreationController : Controller
    {
        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        protected readonly IOperatorsManager _operatorsManager;
        private readonly IHostEnviromentConnection _hostEnviroment;
        protected readonly ITestsContainer _testsContainer;
        private readonly IFactory<MutantsSavingController> _mutantsSavingFactory;
        private readonly IFileManager _fileManager;

        protected readonly CommonServices _svc;

        protected readonly ITypesManager _typesManager;

        protected readonly CreationViewModel _viewModel;


        public MutationSessionChoices Result { get; protected set; }

        public CreationController(
            CreationViewModel viewModel,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            IHostEnviromentConnection hostEnviroment,
            ITestsContainer testsContainer,
            IFactory<MutantsSavingController> mutantsSavingFactory,
            IFileManager fileManager,
            CommonServices svc)
        {
            _viewModel = viewModel;

            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _hostEnviroment = hostEnviroment;
            _testsContainer = testsContainer;
            _mutantsSavingFactory = mutantsSavingFactory;
            _fileManager = fileManager;
            _svc = svc;


            _viewModel.CommandCreateMutants = new SmartCommand(AcceptChoices,
                () => _viewModel.TypesTreeMutate.Assemblies != null && _viewModel.TypesTreeMutate.Assemblies.Count != 0
                      && _viewModel.TypesTreeToTest.TestAssemblies!=null && _viewModel.TypesTreeToTest.TestAssemblies.Count != 0
                      && _viewModel.MutationsTree.MutationPackages.Count != 0)
                .UpdateOnChanged(_viewModel.TypesTreeMutate, _ => _.Assemblies)
                .UpdateOnChanged(_viewModel.TypesTreeToTest, _ => _.TestAssemblies)
                .UpdateOnChanged(_viewModel.MutationsTree, _ => _.MutationPackages);


            _viewModel.CommandWriteMutants = new SmartCommand(GenerateMutants,
                () => _viewModel.TypesTreeMutate.Assemblies != null && _viewModel.TypesTreeMutate.Assemblies.Count != 0
                      && _viewModel.MutationsTree.MutationPackages.Count != 0)
                .UpdateOnChanged(_viewModel.TypesTreeMutate, _ => _.Assemblies)
                .UpdateOnChanged(_viewModel.MutationsTree, _ => _.MutationPackages);
        }

        public void SetMutationConstraints(ClassAndMethod classAndMethod)
        {

        }

        public void Run(ClassAndMethod classAndMethod = null)
        {
            bool loadError;
            var originalFilesList = _fileManager.CopyOriginalFiles(out loadError);
            var originalFilesListForTests = _fileManager.CopyOriginalFiles(out loadError);
            if (loadError || originalFilesListForTests.Count == 0)
            {
                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), null);
            }

            _viewModel.ProjectPaths = _hostEnviroment.GetProjectPaths().ToList();

            _svc.Threading.ScheduleAsync(() => _operatorsManager.LoadOperators(),
                packages => _viewModel.MutationsTree.MutationPackages
                    = new ReadOnlyCollection<PackageNode>(packages));

            
            List<ClassAndMethod> coveredTests = null;
            var t = Task.Run(() => _typesManager.GetTypesFromAssemblies(originalFilesList, classAndMethod,
                out coveredTests).CastTo<object>());
            var task = Task.Run(() => _testsContainer.LoadTests(
                originalFilesListForTests.AsStrings()).ToList().CastTo<object>());

            Task.WhenAll(t, task).ContinueWith( 
                (Task<object[]> result) =>
                {
                    if(result.Exception!= null)
                    {
                        _log.Error(result.Exception);
                        _svc.Threading.PostOnGui(() =>
                        {
                            if (result.Exception.Flatten().InnerException is TestWasSelectedToMutateException)
                            {
                                _svc.Logging.ShowError(UserMessages.ErrorBadMethodSelected(), _viewModel.View);
                            }
                            else if (result.Exception.Flatten().InnerException is StrongNameSignedAssemblyException)
                            {
                                _svc.Logging.ShowError(UserMessages.ErrorStrongNameSignedAssembly(), _viewModel.View);
                            }
                            else if (result.Exception.Flatten().InnerException is TestsLoadingException)
                            {
                                _svc.Logging.ShowError(UserMessages.ErrorTestsLoading(), _viewModel.View);
                            }
                            else
                            {
                                _svc.Logging.ShowError(result.Exception, _viewModel.View);
                            }
                            _viewModel.Close();
                        });
                    }
                    else
                    {
                        var assemblies = (IList<AssemblyNode>)result.Result[0];
                        var tests = (IList<TestNodeAssembly>)result.Result[1];

                        assemblies = assemblies.Where(a => a.Children.Count > 0).ToList();
                        
                        SelectOnlyCovered(tests, coveredTests);
                        _svc.Threading.PostOnGui(() =>
                        {
                            if (_typesManager.IsAssemblyLoadError)
                            {
                                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), _viewModel.View);
                            }
                            _viewModel.TypesTreeMutate.AssembliesPaths = originalFilesList.AsStrings().ToList();

                            if(classAndMethod != null)
                            {
                                var allTypeTreeNodes = assemblies
                                    .Cast<CheckedNode>()
                                    .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>(),
                                    n => n.IsIncluded == null || n.IsIncluded == true)
                                    .Cast<MutationNode>();
                                foreach (var typeTreeNode in allTypeTreeNodes)
                                {
                                    typeTreeNode.IsExpanded = true;
                                }

                                var allTests = tests
                                    .Cast<CheckedNode>()
                                    .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>(),
                                    n => n.IsIncluded == null || n.IsIncluded == true)
                                    .Cast<TestTreeNode>();
                                foreach (var testNode in allTests)
                                {
                                    testNode.IsExpanded = true;
                                }
                            }
                            
                            _viewModel.TypesTreeMutate.Assemblies = new ReadOnlyCollection<AssemblyNode>(assemblies);
                            _viewModel.TypesTreeToTest.TestAssemblies = new ReadOnlyCollection<TestNodeAssembly>(tests);

                        });
                    }
                });

            _viewModel.ShowDialog();
        }

        private void SelectOnlyCovered(IList<TestNodeNamespace> tests, List<ClassAndMethod> coveredTests)
        {
            if (coveredTests != null)
            {
                foreach (var testNodeNamespace in tests)
                {
                    testNodeNamespace.IsIncluded = false;
                }
                var toSelect = tests.Cast<CheckedNode>().SelectManyRecursive(n => n.Children, leafsOnly: true)
                    .OfType<TestNodeMethod>()
                    .Where(t => coveredTests.Select(_=>_.ClassName).Contains(t.ContainingClassFullName)
                                &&  coveredTests.Select(_=>_.MethodName).Contains(t.Name));
                foreach (var testNodeMethod in toSelect)
                {
                    testNodeMethod.IsIncluded = true;
                }
            }
        }


//        public void Run()
//        {
//        
//            bool loadError;
//            var originalFilesList = _fileManager.CopyOriginalFiles(out loadError);
//            var originalFilesListForTests = _fileManager.CopyOriginalFiles(out loadError);
//            if (loadError)
//            {
//                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), null);
//            }
//            _viewModel.ProjectPaths = _hostEnviroment.GetProjectPaths().ToList();
//
//            _svc.Threading.ScheduleAsync(()=> _operatorsManager.LoadOperators(),
//                packages => _viewModel.MutationsTree.MutationPackages 
//                    = new ReadOnlyCollection<PackageNode>(packages));
//
//            _svc.Threading.ScheduleAsync(() => _typesManager.GetTypesFromAssemblies(originalFilesList),
//                assemblies =>
//                {
//                    _viewModel.TypesTreeMutate.Assemblies =  new ReadOnlyCollection<AssemblyNode>(assemblies);
//                    _viewModel.TypesTreeMutate.AssembliesPaths = originalFilesList.AsStrings().ToList();
//                    if (_typesManager.IsAssemblyLoadError)
//                    {
//                        _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(),  _viewModel.View);
//                    }
//                });
//            _svc.Threading.ScheduleAsync(() => _testsContainer.LoadTests(
//                originalFilesListForTests.AsStrings()),
//               tests =>
//               {
//                   _viewModel.TypesTreeToTest.Namespaces = new ReadOnlyCollection<TestNodeNamespace>(tests.ToList());
//
//                
//               });
//            _viewModel.ShowDialog();
//    
//        }

        protected void AcceptChoices()
        {
            Result = new MutationSessionChoices
            {
                SelectedOperators = _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                    .Where(oper => (bool)oper.IsIncluded).Select(n => n.Operator).ToList(),
                Assemblies = _viewModel.TypesTreeMutate.Assemblies,
                AssembliesPaths = _viewModel.TypesTreeMutate.AssembliesPaths,
                ProjectPaths = _viewModel.ProjectPaths.ToList(),
                Filter = _typesManager.CreateFilterBasedOnSelection(_viewModel.TypesTreeMutate.Assemblies),
                SelectedTests = _testsContainer.GetIncludedTests(_viewModel.TypesTreeToTest.TestAssemblies),
                MutantsCreationOptions = _viewModel.MutantsCreation.Options,
                MutantsTestingOptions = _viewModel.MutantsTesting.Options,
                MutantsCreationFolderPath = _viewModel.MutantsGenerationPath
            };
            _viewModel.Close();
        }

        public void GenerateMutants()
        {
            MutantsSavingController mutantsSavingController = _mutantsSavingFactory.Create();
            mutantsSavingController.Run();
            if(mutantsSavingController.Result != null)
            {
                _viewModel.MutantsGenerationPath = mutantsSavingController.Result;
                AcceptChoices();
            }
        }


        public bool HasResults
        {
            get
            {
                return Result != null;
            }
        }

    }
}