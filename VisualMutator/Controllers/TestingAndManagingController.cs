namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;
    using CommonUtilityInfrastructure.WpfUtils.Messages;

    using VisualMutator.Controllers.EventMessages;
    using VisualMutator.Infrastructure.Factories;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.ViewModels;
    using VisualMutator.Views.Abstract;

    using log4net;

    #endregion

    public class TestingAndManagingController : Controller, IHandler<LoadLastCreatedMutantEventArgs>
    {
        private BasicCommand _commandRunTests;

        private readonly IMutantsContainer _mutantsContainer;

        private readonly ITestsContainer _testsContainer;

        private readonly Services _services;

        private readonly IFactory<MutantsManagementViewModel> _mutantsManagementfactory;

        private readonly UnitTestsViewModel _viewModel;


        private BasicCommand _commandReloadTestList;
   
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TestingAndManagingController(
            IUnitTestsView view,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer,
            Services services,
            IFactory<MutantsManagementViewModel> mutantsManagementfactory)
        {
   
        
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _services = services;
            _mutantsManagementfactory = mutantsManagementfactory;

            _viewModel = new UnitTestsViewModel(view, _mutantsContainer.GeneratedMutants);
            InitViewModel();

            _services.EventPassing.Subscribe(this);
        }



        private void InitViewModel()
        {
            _commandRunTests = new BasicCommand(RunTests, () =>
               _viewModel.SelectedMutant != null && !_viewModel.AreTestsRunning && !_viewModel.AreTestsLoading);
            _commandRunTests.UpdateOnChanged(_viewModel, () => _viewModel.SelectedMutant);
            _commandRunTests.UpdateOnChanged(_viewModel, () => _viewModel.AreTestsRunning);
            _commandRunTests.UpdateOnChanged(_viewModel, () => _viewModel.AreTestsLoading);
            _viewModel.CommandRunTests = _commandRunTests;


            _commandReloadTestList = new BasicCommand(RefreshTestList, () => _viewModel.SelectedMutant != null);
            _commandReloadTestList.ExecuteOnChanged(_viewModel, () => _viewModel.SelectedMutant);

          //  _viewModel.CommandDeleteMutant = new BasicCommand(DeleteMutant, () => _viewModel.SelectedMutant != null);
         //   _viewModel.CommandDeleteMutant.UpdateOnChanged(_viewModel, () => _viewModel.SelectedMutant);


            _viewModel.EventListeners.AddCollectionChangedEventHandler(_viewModel.Mutants, ()=>
            {
                if (_viewModel.SelectedMutant == null)
                {
                    _viewModel.SelectedMutant = _viewModel.Mutants.LastOrDefault();
                }  
            });
            _viewModel.CommandManageMutants = new BasicCommand(ManageMutants);
        }
        public void ManageMutants()
        {
            var vm = _mutantsManagementfactory.Create();
           // vm.CommandRemove = new BasicCommand(() => DeleteMutant(vm.SelectedMutant));
         //   vm.CommandRemoveAll = new BasicCommand(DeleteAllMutants);
            vm.Show();
        }

        public void Initialize()
        {
           
        }
        public void Deactivate()
        {
            _testsContainer.UnloadTests();
        }
        public void RefreshTestList()
        {

            _viewModel.AreTestsLoading = true;
            _viewModel.TestNamespaces.Clear();
            /*
            _services.Threading.ScheduleAsync(
                 () =>
                {
                    return _testsContainer.LoadTests(_viewModel.SelectedMutant);
                },
                onGui: result =>
                {
                    _viewModel.TestNamespaces.ReplaceRange(result.TestsRootNode.Children.Cast<TestNodeNamespace>());
                },
                onFinally: () => _viewModel.AreTestsLoading = false);
            */
        }
       

        public void RunTests()
        {

            _viewModel.AreTestsRunning = true;

           
         //   _services.Threading.ScheduleAsync(
       //         () => _testsContainer.RunTests(),
         //       onFinally: () => _viewModel.AreTestsRunning = false);


        }
        /*
        public void DeleteMutant()
        {
            DeleteMutant(_viewModel.SelectedMutant);
        }
        
        public void DeleteMutant(StoredMutantInfo mutant)
        {
            if (_testsContainer.CurrentMutant == mutant)
            {
                _testsContainer.UnloadTests();
            }

        
            if (_viewModel.SelectedMutant == mutant)
            {
                _viewModel.TestNamespaces.Clear();
                _viewModel.SelectedMutant = _viewModel.Mutants.LastOrDefault();
            }

            _mutantsContainer.DeleteMutant(mutant);
        }
       

        public void DeleteAllMutants()
        {
            _testsContainer.UnloadTests();
            _mutantsContainer.DeleteAllMutants();

        }
        */
       
        public void Handle(LoadLastCreatedMutantEventArgs message)
        {
            _viewModel.SelectedMutant = _viewModel.Mutants.LastOrDefault();
        }



        public UnitTestsViewModel UnitTestsVm
        {
            get
            {
                return _viewModel;
            }
        }

       
    }
}