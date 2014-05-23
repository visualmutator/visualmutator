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

    public static class TupleExtensions
    {
        public static async Task<Tuple<T1, T2, T3, T4>> WhenAll<T1, T2, T3, T4>
            (this Tuple<Task<T1>, Task<T2>, Task<T3>, Task<T4>> tasks)
        {
            await Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4);
            return Tuple.Create(tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result);
        }
        public static async Task<Tuple<T1, T2, T3>> WhenAll<T1, T2, T3>
            (this Tuple<Task<T1>, Task<T2>, Task<T3>> tasks)
        {
            await Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3);
            return Tuple.Create(tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result);
        }
        public static async Task<Tuple<T1, T2>> WhenAll<T1, T2>
            (this Tuple<Task<T1>, Task<T2>> tasks)
        {
            await Task.WhenAll(tasks.Item1, tasks.Item2);
            return Tuple.Create(tasks.Item1.Result, tasks.Item2.Result);
        }
    }

    #endregion
    public class SessionCreator
    {
        private readonly ITypesManager _typesManager;
        private readonly IOperatorsManager _operatorsManager;
        private readonly CommonServices _svc;
        private readonly IReporting _reporting;

        public SessionCreator(
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            CommonServices svc,
            IReporting reporting)
        {
            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _svc = svc;
            _reporting = reporting;
            Events = new Subject<object>();
        }

        public Subject<object> Events { get; set; }

        public async Task<List<MethodIdentifier>> FindCoveringTests(Task<IList<IModule>> assembliesTask, ICodePartsMatcher matcher)
        {
            var finder = new CoveringTestsFinder();
            IList<IModule> modules = await assembliesTask;
            return await Task.WhenAll(modules.Select(module =>
                Task.Run(() => finder.FindCoveringTests(module, matcher))))
                    .ContinueWith(t => t.Result.Flatten().ToList());
        }
        public async Task<OperatorPackagesRoot> GetOperators()
        {
            try
            {
                OperatorPackagesRoot root = await _operatorsManager.GetOperators();
                return root;
            }
            catch (Exception e)
            {
                _reporting.LogError(e.ToString());
                throw;
            }
            //Events.OnNext(root);
            
        }

        public async Task<List<TestNodeAssembly>> BuildTestTree(Task<List<MethodIdentifier>> coveringTask, Task<object> testsTask, bool constrainedMutation)
        {

            var result = await Tuple.Create(coveringTask, testsTask).WhenAll();

            var coveringTests = result.Item1;
            var testsRootNode = (TestsRootNode)result.Item2;

            if (constrainedMutation)
            {
                SelectOnlyCoveredTests(testsRootNode, coveringTests);
            }

            if (_typesManager.IsAssemblyLoadError)
            {
                _reporting.LogWarning(UserMessages.WarningAssemblyNotLoaded());
            }
            if (constrainedMutation)
            {
                ExpandLoneNodes(testsRootNode);
            }
            //Events.OnNext(testsRootNode.TestNodeAssemblies.ToList());
            return testsRootNode.TestNodeAssemblies.ToList();
        }

        public async Task<List<AssemblyNode>> BuildAssemblyTree(Task<IList<IModule>> assembliesTask,
            bool constrainedMutation, ICodePartsMatcher matcher)
        {
            var result = await assembliesTask;
            IList<IModule> modules = (IList<IModule>)result;
            var assemblies = _typesManager.CreateNodesFromAssemblies(modules, matcher)
                .Where(a => a.Children.Count > 0).ToList();

            if (constrainedMutation)
            {
                var root = new CheckedNode("");
                root.Children.AddRange(assemblies);
                ExpandLoneNodes(root);
            }
            if(assemblies.Count == 0)
            {
                throw new InvalidOperationException(UserMessages.ErrorNoFilesToMutate());
            }
          //  _reporting.LogError(UserMessages.ErrorNoFilesToMutate());
            return assemblies;
            //Events.OnNext(assemblies);
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


    }
    public class AutoCreationController 
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly SessionConfiguration _sessionConfiguration;
        private readonly IOptionsManager _optionsManager;
        private readonly IFactory<SessionCreator> _sessionCreatorFactory;

        private readonly CommonServices _svc;
        private readonly CreationViewModel _viewModel;
        private readonly ITypesManager _typesManager;

        public MutationSessionChoices Result { get; protected set; }


        public AutoCreationController(
            CreationViewModel viewModel,
            ITypesManager typesManager,
            SessionConfiguration sessionConfiguration,
            IOptionsManager optionsManager,
            IFactory<SessionCreator> sessionCreatorFactory,
            CommonServices svc)
        {
            _viewModel = viewModel;
            _typesManager = typesManager;

            _sessionConfiguration = sessionConfiguration;
            _optionsManager = optionsManager;
            _sessionCreatorFactory = sessionCreatorFactory;
            _svc = svc;

            
        }

       

        public async Task<MutationSessionChoices> Run(MethodIdentifier singleMethodToMutate)
        {
            SessionCreationWindowShowTime = DateTime.Now;

            if(_sessionConfiguration.AssemblyLoadProblem)
            {
                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), null);
            }

            var r = new Reporting(_svc.Logging, _viewModel.View);

            SessionCreator sessionCreator = _sessionCreatorFactory.CreateWithParams(r);

           // sessionCreator.Events.OfType<List<TestNodeAssembly>>()

            ICodePartsMatcher matcher = new CciMethodMatcher(singleMethodToMutate);
            bool constrainedMutation = true;

           
            Task<IList<IModule>> assembliesTask = _sessionConfiguration.LoadAssemblies();

            Task<List<MethodIdentifier>> coveringTask = sessionCreator.FindCoveringTests(assembliesTask, matcher);

            Task<object> testsTask = _sessionConfiguration.LoadTests();

            var t1 = sessionCreator.GetOperators();

            var t2 = sessionCreator.BuildAssemblyTree(assembliesTask, constrainedMutation, matcher);


            var t3 = sessionCreator.BuildTestTree(coveringTask, testsTask, constrainedMutation);

            t1.ContinueWith(task =>
            {
                _viewModel.MutationsTree.MutationPackages
                    = new ReadOnlyCollection<PackageNode>(task.Result.Packages);
            }, TaskContinuationOptions.NotOnFaulted);

            t2.ContinueWith(task =>
            {
                _viewModel.TypesTreeMutate.Assemblies = new ReadOnlyCollection<AssemblyNode>(task.Result);
            }, TaskContinuationOptions.NotOnFaulted);

            t3.ContinueWith(task =>
            {
                _viewModel.TypesTreeToTest.TestAssemblies
                                = new ReadOnlyCollection<TestNodeAssembly>(task.Result);
            }, TaskContinuationOptions.NotOnFaulted);

            try
            {
                await Task.WhenAll(t1, t2, t3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
            //  await 
            return Result;
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