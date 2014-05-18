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
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly IOperatorsManager _operatorsManager;
        private readonly ITypesManager _typesManager;

        private readonly SessionConfiguration _sessionConfiguration;
        private readonly IOptionsManager _optionsManager;

        private readonly CommonServices _svc;
        private readonly IDispatcherExecute _dispatcher;
        private readonly CreationViewModel _viewModel;


        public MutationSessionChoices Result { get; protected set; }


        public CreationController(
            IDispatcherExecute dispatcher,
            CreationViewModel viewModel,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            SessionConfiguration sessionConfiguration,
            IOptionsManager optionsManager,
            CommonServices svc)
        {
            _dispatcher = dispatcher;
            _viewModel = viewModel;

            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _sessionConfiguration = sessionConfiguration;
            _optionsManager = optionsManager;
            _svc = svc;


            _viewModel.CommandCreateMutants = new SmartCommand(AcceptChoices,
                () => _viewModel.TypesTreeMutate.Assemblies != null && _viewModel.TypesTreeMutate.Assemblies.Count != 0
                      && _viewModel.TypesTreeToTest.TestAssemblies!=null && _viewModel.TypesTreeToTest.TestAssemblies.Count != 0
                      && _viewModel.MutationsTree.MutationPackages.Count != 0)
                .UpdateOnChanged(_viewModel.TypesTreeMutate, _ => _.Assemblies)
                .UpdateOnChanged(_viewModel.TypesTreeToTest, _ => _.TestAssemblies)
                .UpdateOnChanged(_viewModel.MutationsTree, _ => _.MutationPackages);


        }

        public void Run(MethodIdentifier singleMethodToMutate = null)
        {
            SessionCreationWindowShowTime = DateTime.Now;

            if(_sessionConfiguration.AssemblyLoadProblem)
            {
                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), null);
            }
            
//
//            _fileManager.Initialize();
//
//            ProjectFilesClone originalFilesList = _fileManager.CreateClone("Mutants");
//
//            _whiteCache.Initialize(_optionsManager.ReadOptions(), 
//                originalFilesList.Assemblies.AsStrings().ToList());
//
//            ProjectFilesClone originalFilesListForTests = _fileManager.CreateClone("Tests");
//            if (originalFilesList.IsIncomplete || originalFilesListForTests.IsIncomplete
//                || originalFilesListForTests.Assemblies.Count == 0)
//            {
//                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), null);
//            }


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
            

            _operatorsManager.GetOperators().ContinueWith(t =>
            {
                _viewModel.MutationsTree.MutationPackages
                    = new ReadOnlyCollection<PackageNode>(t.Result.Packages);
            }, _dispatcher.GuiScheduler);

           // _svc.Threading.ScheduleAsync(() => _operatorsManager.LoadOperators(),
            //    root => _viewModel.MutationsTree.MutationPackages
              //      = new ReadOnlyCollection<PackageNode>(root.Packages));
            var finder = new CoveringTestsFinder();
           
           // List<MethodIdentifier> coveredTests = null;
            Task<IList<IModule>> assembliesTask = _sessionConfiguration.LoadAssemblies();


            var coveringTask = assembliesTask.ContinueWith((task) =>
            {
                
                IList<IModule> modules = (IList<IModule>)task.Result;
                return Task.WhenAll(modules.Select(module =>
                    Task.Run(() => finder.FindCoveringTests(module, matcher))))
                    .ContinueWith(t =>
                    {
                        return (object) t.Result.Flatten().ToList();
                    });
                
            }, TaskContinuationOptions.NotOnFaulted).Unwrap();
            

            assembliesTask.ContinueWith((result) =>
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
                        _viewModel.TypesTreeMutate.Assemblies = new ReadOnlyCollection<AssemblyNode>(assemblies);
                    });
                }
            }).ContinueWith(CheckError);


            var testsTask = _sessionConfiguration.LoadTests();

            Task.WhenAll(coveringTask, testsTask).ContinueWith( 
                (Task<object[]> result) =>
                {
                    if (result.Exception == null)
                    {
                        var coveringTests = (List<MethodIdentifier>)result.Result[0];
                        var testsRootNode = (TestsRootNode)result.Result[1];

                        if (constrainedMutation)
                        {
                            

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

        public DateTime SessionCreationWindowShowTime { get; set; }

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

        private void SelectOnlyCoveredTests(TestsRootNode rootNode, List<MethodIdentifier> coveredTests)
        {
            rootNode.IsIncluded = false;
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
                Filter = _typesManager.CreateFilterBasedOnSelection(_viewModel.TypesTreeMutate.Assemblies),
                TestAssemblies = _viewModel.TypesTreeToTest.TestAssemblies,
                MutantsCreationOptions = _viewModel.MutantsCreation.Options,
                MutantsTestingOptions = _viewModel.MutantsTesting.Options,
                MainOptions = _optionsManager.ReadOptions(),
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

      
    }
}