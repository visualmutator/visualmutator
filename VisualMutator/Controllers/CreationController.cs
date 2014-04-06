namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Infrastructure;
    using log4net;
    using Microsoft.Cci;
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
        protected readonly ITypesManager _typesManager;
        private readonly TestsLoader _testsLoader;

        private readonly IHostEnviromentConnection _hostEnviroment;
        private readonly IFileSystemManager _fileManager;
        private readonly IWhiteCache _whiteCache;

        protected readonly CommonServices _svc;
        private readonly IFactory<MutantsSavingController> _mutantsSavingFactory;
        private readonly IBindingFactory<SessionController> _sessionFactory;
        private readonly IDispatcherExecute _dispatcher;
        protected readonly CreationViewModel _viewModel;


        public MutationSessionChoices Result { get; protected set; }


        public CreationController(
            IDispatcherExecute dispatcher,
            CreationViewModel viewModel,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            IHostEnviromentConnection hostEnviroment,
            TestsLoader testsLoader,
            IFactory<MutantsSavingController> mutantsSavingFactory,
            IBindingFactory<SessionController> sessionFactory,
            IFileSystemManager fileManager,
            IWhiteCache whiteCache,
            CommonServices svc)
        {
            _dispatcher = dispatcher;
            _viewModel = viewModel;

            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _hostEnviroment = hostEnviroment;
            _testsLoader = testsLoader;
            _mutantsSavingFactory = mutantsSavingFactory;
            _sessionFactory = sessionFactory;
            _fileManager = fileManager;
            _whiteCache = whiteCache;
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

        public void SetMutationConstraints(MethodIdentifier methodIdentifier)
        {

        }

        public void Run(MethodIdentifier singleMethodToMutate = null)
        {
            bool constrainedMutation = false;
            ICodePartsMatcher matcher;
            if(singleMethodToMutate != null)
            {
                matcher = new CciMethodMatcher(singleMethodToMutate);
                constrainedMutation = true;
            }
            else
            {
                matcher = new AllMatcher();
            }
            

            _fileManager.Initialize();

            ProjectFilesClone originalFilesList = _fileManager.CreateClone("Mutants");

            _whiteCache.Initialize(originalFilesList.Assemblies.AsStrings().ToList());

            ProjectFilesClone originalFilesListForTests = _fileManager.CreateClone("Tests");
            if (originalFilesList.IsIncomplete || originalFilesListForTests.IsIncomplete 
                || originalFilesListForTests.Assemblies.Count == 0)
            {
                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), null);
            }
            
            _viewModel.ProjectPaths = _hostEnviroment.GetProjectPaths().ToList();

            _operatorsManager.GetOperators().ContinueWith(t =>
            {
                _viewModel.MutationsTree.MutationPackages
                    = new ReadOnlyCollection<PackageNode>(t.Result.Packages);
            }, _dispatcher.GuiScheduler);

           // _svc.Threading.ScheduleAsync(() => _operatorsManager.LoadOperators(),
            //    root => _viewModel.MutationsTree.MutationPackages
              //      = new ReadOnlyCollection<PackageNode>(root.Packages));

           
           // List<MethodIdentifier> coveredTests = null;
            var assembliesTask = Task.Run(() => _typesManager.LoadAssemblies(
                originalFilesList.Assemblies).CastTo<object>());

            var testsTask = Task.Run(() => _testsLoader.LoadTests(
                originalFilesListForTests.Assemblies.AsStrings().ToList()).CastTo<object>());

            assembliesTask.ContinueWith((Task<object> result) =>
            {
                if (result.Exception == null)
                {
                    IList<IModule> modules = (IList<IModule>) result.Result;
                    var assemblies = _typesManager.CreateNodesFromAssemblies(modules, matcher)
                                .Where(a => a.Children.Count > 0).ToList();

                    if (constrainedMutation)
                    {
                        var root = new CheckedNode("");
                        root.Children.AddRange(assemblies);
                        ExpandLoneNodes(root);
                    }
                    _svc.Threading.PostOnGui(() =>
                    {
                        _viewModel.TypesTreeMutate.AssembliesPaths = originalFilesList.Assemblies.AsStrings().ToList();

                        _viewModel.TypesTreeMutate.Assemblies = new ReadOnlyCollection<AssemblyNode>(assemblies);
                    });
                }
            }).ContinueWith(CheckError);

            var finder = new CoveringTestsFinder();
            Task.WhenAll(assembliesTask, testsTask).ContinueWith( 
                (Task<object[]> result) =>
                {
                    if (result.Exception == null)
                    {
                        var testsRootNode = (TestsRootNode)result.Result[1];
                        var modules = (IList<IModule>)result.Result[0];

                        if (constrainedMutation)
                        {

                            var t = Task.WhenAll(modules.Select(module =>
                                Task.Run(() => finder.FindCoveringTests(module, matcher))));
                            List<MethodIdentifier> coveringTests = t.Result.Flatten().ToList();


                            SelectOnlyCoveredTests(testsRootNode, coveringTests);
                        }

                        _svc.Threading.PostOnGui(() =>
                        {
                            if (_typesManager.IsAssemblyLoadError)
                            {
                                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), _viewModel.View);
                            }
                            if (singleMethodToMutate != null)
                            {
                                ExpandLoneNodes(testsRootNode);
                            }

                            _viewModel.TypesTreeToTest.TestAssemblies 
                                = new ReadOnlyCollection<TestNodeAssembly>(testsRootNode.TestNodeAssemblies.ToList());

                        });
                    }
                }).ContinueWith(CheckError);

            _viewModel.ShowDialog();
        }

        private void CheckError(Task result)
        {
            if (result.Exception != null)
            {
                ShowErrorAndExit(result.Exception);
            }
                
        }

        private void ShowErrorAndExit(AggregateException exception)
        {
            _log.Error(exception);
            _svc.Threading.PostOnGui(() =>
            {
                ShowError(exception);
                _viewModel.Close();
            });
        }

        private void ShowError(AggregateException exc)
        {
            Exception innerException = exc.Flatten().InnerException;
            if (innerException is TestWasSelectedToMutateException)
            {
                _svc.Logging.ShowError(UserMessages.ErrorBadMethodSelected(), _viewModel.View);
            }
            else if (innerException is StrongNameSignedAssemblyException)
            {
                _svc.Logging.ShowError(UserMessages.ErrorStrongNameSignedAssembly(), _viewModel.View);
            }
            else if (innerException is TestsLoadingException)
            {
                _svc.Logging.ShowError(UserMessages.ErrorTestsLoading(), _viewModel.View);
            }
            else
            {
                _svc.Logging.ShowError(exc, _viewModel.View);
            }
        }

        private void ExpandLoneNodes(CheckedNode tests)
        {
            var allTests = tests.Children
                .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>(),
                    n => n.IsIncluded == null || n.IsIncluded == true)
                .Cast<IExpandableNode>();
            foreach (var testNode in allTests)
            {
                testNode.IsExpanded = true;
            }
        }

        public void GenerateMutants()
        {
            MutantsSavingController mutantsSavingController = _mutantsSavingFactory.Create();
            mutantsSavingController.Run();
            if (mutantsSavingController.Result != null)
            {
                _viewModel.MutantsGenerationPath = mutantsSavingController.Result;
                AcceptChoices();
            }
        }

        private void SelectOnlyCoveredTests(TestsRootNode rootNode, List<MethodIdentifier> coveredTests)
        {
            rootNode.IsIncluded = false;
            var se = rootNode.Children.SelectManyRecursive(n => n.Children, leafsOnly: true)
                .OfType<TestNodeMethod>()
                .Select(m => m.Identifier)
                .ToList();
            se.ToString();
            var toSelect = rootNode.Children.SelectManyRecursive(n => n.Children, leafsOnly: true)
                .OfType<TestNodeMethod>()
                .Where(t => coveredTests.Contains(t.Identifier));
            foreach (var testNodeMethod in toSelect)
            {
                testNodeMethod.IsIncluded = true;
            }
        }


      


        protected void AcceptChoices()
        {
            Result = new MutationSessionChoices
            {
                SelectedOperators = _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                    .Where(oper => (bool)oper.IsIncluded).Select(n => n.Operator).ToList(),
                AssembliesPaths = _viewModel.TypesTreeMutate.AssembliesPaths,
                ProjectPaths = _viewModel.ProjectPaths.ToList(),
                Filter = _typesManager.CreateFilterBasedOnSelection(_viewModel.TypesTreeMutate.Assemblies),
                TestAssemblies = _viewModel.TypesTreeToTest.TestAssemblies,
                MutantsCreationOptions = _viewModel.MutantsCreation.Options,
                MutantsTestingOptions = _viewModel.MutantsTesting.Options,
                MutantsCreationFolderPath = _viewModel.MutantsGenerationPath
            };
            _viewModel.Close();
        }

        public bool HasResults
        {
            get
            {
                return Result != null;
            }
        }

        public SessionController CreateSession()
        {
            return _sessionFactory.CreateWithBindings(Result);
        }
    }
}