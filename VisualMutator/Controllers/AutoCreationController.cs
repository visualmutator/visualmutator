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
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using log4net;
    using Microsoft.Cci;
    using Model;
    using Model.CoverageFinder;
    using Model.Exceptions;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.TestsTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using UsefulTools.Wpf;
    using ViewModels;

    #endregion

    public class AutoCreationController 
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SessionConfiguration _sessionConfiguration;
        private readonly OptionsModel _options;
        private readonly IFactory<SessionCreator> _sessionCreatorFactory;
        private readonly IDispatcherExecute _execute;
        private readonly CommonServices _svc;
        private readonly IDispatcherExecute _dispatcher;
        private readonly CreationViewModel _viewModel;
        private readonly ITypesManager _typesManager;
        private List<CciModuleSource> _whiteSource;

        public MutationSessionChoices Result { get; protected set; }

        TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
        private DateTime _sessionCreationWindowShowTime;

        public AutoCreationController(
            IDispatcherExecute dispatcher,
            CreationViewModel viewModel,
            ITypesManager typesManager,
            SessionConfiguration sessionConfiguration,
            OptionsModel options,
            IFactory<SessionCreator> sessionCreatorFactory,
            IDispatcherExecute execute,
            CommonServices svc)
        {
            _dispatcher = dispatcher;
            _viewModel = viewModel;
            _typesManager = typesManager;
            _sessionConfiguration = sessionConfiguration;
            _options = options;
            _sessionCreatorFactory = sessionCreatorFactory;
            _execute = execute;
            _svc = svc;
            
            _viewModel.CommandCreateMutants = new SmartCommand(CommandOk,
               () => _viewModel.TypesTreeMutate.Assemblies != null && _viewModel.TypesTreeMutate.Assemblies.Count != 0
                     && _viewModel.TypesTreeToTest.TestAssemblies != null && _viewModel.TypesTreeToTest.TestAssemblies.Count != 0
                     && _viewModel.MutationsTree.MutationPackages.Count != 0)
                   .UpdateOnChanged(_viewModel.TypesTreeMutate, _ => _.Assemblies)
                   .UpdateOnChanged(_viewModel.TypesTreeToTest, _ => _.TestAssemblies)
                   .UpdateOnChanged(_viewModel.MutationsTree, _ => _.MutationPackages);
        }

        public DateTime SessionCreationWindowShowTime
        {
            get; set;
        }

        public async Task<MutationSessionChoices> Run(MethodIdentifier singleMethodToMutate = null, List<string> testAssemblies = null, bool auto = false)
        {
            _sessionCreationWindowShowTime = DateTime.Now;


           
            SessionCreator sessionCreator = _sessionCreatorFactory.Create();

            Task<List<CciModuleSource>> assembliesTask = _sessionConfiguration.LoadAssemblies();

        
           // Task<List<MethodIdentifier>> coveringTask = sessionCreator.FindCoveringTests(assembliesTask, matcher);

            Task<TestsRootNode> testsTask = _sessionConfiguration.LoadTests();


            ITestsSelectStrategy testsSelector;
            bool constrainedMutation = false;
            ICodePartsMatcher matcher;
            if (singleMethodToMutate != null)
            {
                matcher = new CciMethodMatcher(singleMethodToMutate);
                testsSelector = new CoveringTestsSelectStrategy(assembliesTask, matcher, testsTask);
                constrainedMutation = true;
            }
            else
            {
                testsSelector = new AllTestsSelectStrategy(testsTask);
                matcher = new AllMatcher();
            }

            var testsSelecting = testsSelector.SelectTests(testAssemblies);

            var t1 = sessionCreator.GetOperators();

            var t2 = sessionCreator.BuildAssemblyTree(assembliesTask, constrainedMutation, matcher);

            var t11 = t1.ContinueWith(task =>
            {
                _viewModel.MutationsTree.MutationPackages
                    = new ReadOnlyCollection<PackageNode>(task.Result.Packages);
            },CancellationToken.None, TaskContinuationOptions.NotOnFaulted, _execute.GuiScheduler);

            var t22 = t2.ContinueWith(task =>
            {
                if (_typesManager.IsAssemblyLoadError)
                {
                    _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded());
                }
                var assembliesToMutate = task.Result.Where(a => !testAssemblies.ToEmptyIfNull().Contains(a.AssemblyPath.Path)).ToList();
                _viewModel.TypesTreeMutate.Assemblies = new ReadOnlyCollection<AssemblyNode>(assembliesToMutate);
                _whiteSource = assembliesTask.Result;
            }, CancellationToken.None, TaskContinuationOptions.NotOnFaulted, _execute.GuiScheduler);

            var t33 = testsSelecting.ContinueWith(task =>
            {
                _viewModel.TypesTreeToTest.TestAssemblies
                                = new ReadOnlyCollection<TestNodeAssembly>(task.Result);
               
            }, CancellationToken.None, TaskContinuationOptions.NotOnFaulted, _execute.GuiScheduler);

              
            try
            {
                var mainTask = Task.WhenAll(t1, t2, testsSelecting, t11, t22, t33).ContinueWith(t =>
                {
                    
                    if (t.Exception != null)
                    {
                        ShowError(t.Exception);
                        _viewModel.Close();
                        tcs.TrySetCanceled();
                    }
                }, _execute.GuiScheduler);

                var wrappedTask = Task.WhenAll(tcs.Task, mainTask);

                if (_sessionConfiguration.AssemblyLoadProblem)
                {
                    new TaskFactory(_dispatcher.GuiScheduler)
                        .StartNew(() =>
                        _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded()));
                }
                return await WaitForResult(auto, wrappedTask);
            }
            catch (Exception e)
            {
                _log.Error(e);
                throw;
            }
        }

        protected void CommandOk()
        {
            if (_viewModel.TypesTreeToTest.TestAssemblies.All(a => a.IsIncluded == false))
            {
                _svc.Logging.ShowError(UserMessages.ErrorNoTestsToRun(), _viewModel.View);
                return;
                //   throw new Exception(UserMessages.ErrorNoTestsToRun());
            }
            tcs.TrySetResult(new object());
            _viewModel.Close();
        }
        private async Task<MutationSessionChoices> WaitForResult(bool auto, Task mainTask)
        {
            _viewModel.ShowDialog(); // blocking if gui
            if (!auto && !tcs.Task.IsCompleted) //CommandOk was not called
            {
                tcs.TrySetCanceled();
            }
            if (auto)
            {
                tcs.TrySetResult(new object());
            }
            await mainTask;
            if (auto)
            {
                if (_viewModel.TypesTreeToTest.TestAssemblies.All(a => a.IsIncluded == false))
                {
                    //_svc.Logging.ShowError(UserMessages.ErrorNoTestsToRun(), _viewModel.View);
                       throw new Exception(UserMessages.ErrorNoTestsToRun());
                }
            }
            
            return AcceptChoices();
        }
        protected MutationSessionChoices AcceptChoices()
        {
            
            return new MutationSessionChoices
            {
                SelectedOperators = _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                    .Where(oper => (bool)oper.IsIncluded).Select(n => n.Operator).ToList(),
                Filter = _typesManager.CreateFilterBasedOnSelection(_viewModel.TypesTreeMutate.Assemblies),
                TestAssemblies = _viewModel.TypesTreeToTest.TestAssemblies,
                SessionCreationWindowShowTime = _sessionCreationWindowShowTime
            };

        }

        
        private void ShowError(Exception exc)
        {
            var aggregate = exc as AggregateException;
            Exception innerException = aggregate == null ? exc : aggregate.Flatten().InnerException;
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
                _svc.Logging.ShowError(UserMessages.ErrorTestsLoading() + " "+innerException, _viewModel.View);
            }
            else
            {
                _svc.Logging.ShowError(innerException, _viewModel.View);
            }
        }



      
    }
}