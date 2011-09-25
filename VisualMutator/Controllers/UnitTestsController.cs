namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure.WpfUtils;
    using CommonUtilityInfrastructure.WpfUtils.Messages;

    using VisualMutator.Controllers.EventMessages;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.ViewModels;
    using VisualMutator.Views.Abstract;

    using log4net;

    #endregion

    public class UnitTestsController : Controller, IHandler<LoadLastCreatedMutantEventArgs>
    {
        private BasicCommand _commandRunTests;

        private readonly IExecute _execute;

        private readonly IEventService _eventService;

        private readonly IMessageService _messageBoxService;

        private readonly IMutantsContainer _mutantsContainer;

        private readonly ITestsContainer _testsContainer;

        private readonly Dictionary<string, TestTreeNode> _testMap;

        private readonly UnitTestsViewModel _viewModel;


        private BasicCommand _reloadTestList;
        private BasicCommand _showTestDetails;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UnitTestsController(
            IUnitTestsView view,
            IMessageService messageBoxService,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer,
            IExecute execute,
            IEventService eventService
            )
        {
   
            _messageBoxService = messageBoxService;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _execute = execute;
            _eventService = eventService;

            _testMap = new Dictionary<string, TestTreeNode>();

            _viewModel = new UnitTestsViewModel(view, _mutantsContainer.GeneratedMutants);
            InitViewModel();

            _eventService.Subscribe(this);
        }



        private void InitViewModel()
        {
            _commandRunTests = new BasicCommand(RunTests, () =>
               _viewModel.SelectedMutant != null && !_viewModel.AreTestsRunning && !_viewModel.AreTestsLoading);
            _commandRunTests.UpdateOnChanged(_viewModel, () => _viewModel.SelectedMutant);
            _commandRunTests.UpdateOnChanged(_viewModel, () => _viewModel.AreTestsRunning);
            _commandRunTests.UpdateOnChanged(_viewModel, () => _viewModel.AreTestsLoading);


            _reloadTestList = new BasicCommand(RefreshTestList, () => _viewModel.SelectedMutant != null);
            _reloadTestList.ExecuteOnChanged(_viewModel, () => _viewModel.SelectedMutant);

            _showTestDetails = new BasicCommand(ShowTestDetails);
            _showTestDetails.ExecuteOnChanged(_viewModel, () => _viewModel.SelectedTestItem);

            _viewModel.CommandDeleteMutant = new BasicCommand(DeleteMutant, () => _viewModel.SelectedMutant != null);
            _viewModel.CommandDeleteMutant.UpdateOnChanged(_viewModel, () => _viewModel.SelectedMutant);


            _viewModel.CommandRunTests = _commandRunTests;

            _viewModel.EventListeners.AddCollectionChangedEventHandler(_viewModel.Mutants, ()=>
            {
                if (_viewModel.SelectedMutant == null)
                {
                    _viewModel.SelectedMutant = _viewModel.Mutants.LastOrDefault();
                }  
            });
           
        }



        public UnitTestsViewModel UnitTestsVm
        {
            get
            {
                return _viewModel;
            }
        }
        public void DeleteMutant()
        {
            _mutantsContainer.DeleteMutant(_viewModel.SelectedMutant);
            _viewModel.SelectedMutant = _viewModel.Mutants.Last();

        }

        public void ShowTestDetails()
        {
            var method = _viewModel.SelectedTestItem as TestNodeMethod;
            if (method != null && method.HasResults)
            {
                _viewModel.ResultText = method.Message;
            }
            else
            {
                _viewModel.ResultText = "";
            }
        }

        public void Initialize()
        {
           
        }
        public void Deactivate()
        {
          
        }
        public void RefreshTestList()
        {

            _viewModel.AreTestsLoading = true;
            _viewModel.TestNamespaces.Clear();
            Task.Factory.StartNew(() =>
            {
                return _testsContainer.LoadTests(_viewModel.SelectedMutant.Assemblies);

            })
            .ContinueWith(prev =>
            {
                try
                {
                    if (prev.Exception != null)
                    {
                        _messageBoxService.ShowError(prev.Exception, _log);
                    }
                    else
                    {
                        _viewModel.TestNamespaces.ReplaceRange(prev.Result);
                    }
                }
                catch (Exception e)
                {
                    _messageBoxService.ShowError(e, _log);
                }
                finally
                {
                    _viewModel.AreTestsLoading = false;
                }
            }, _execute.GuiScheduler);


        }
       

        public void RunTests()
        {

            _viewModel.AreTestsRunning = true;

            
            Task.Factory.StartNew(() =>
            {
                _testsContainer.RunTests();

            }).ContinueWith( prev =>
            {
                if (prev.Exception != null)
                {
                    _messageBoxService.ShowError(prev.Exception, _log);
                }
                _viewModel.AreTestsRunning = false;

            }, _execute.GuiScheduler);


        }

        public void Handle(LoadLastCreatedMutantEventArgs message)
        {
            MutationSession last = _viewModel.Mutants.LastOrDefault();
            if (last != null)
            {
                _viewModel.SelectedMutant = last;
            }
        }
    }
}