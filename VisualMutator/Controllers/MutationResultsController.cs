using System.Drawing;
namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.DependencyInjection;
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Infrastructure;
    using VisualMutator.Model;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.ViewModels;

    using log4net;


    
    public class MutationResultsController : Controller
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MutationResultsViewModel _viewModel;

        private readonly IFactory<MutantsCreationController> _mutantsCreationFactory;

        private readonly IFactory<SessionController> _sessionControllerFactory;

        private readonly IFactory<ResultsSavingController> _resultsSavingFactory;

   //     private readonly IMutantsContainer _mutantsContainer;

   //     private readonly ITestsContainer _testsContainer;

        private readonly MutantDetailsController _mutantDetailsController;

        private readonly CommonServices _commonServices;

        private MutationTestingSession _currentSession;

        private RequestedHaltState? _requestedHaltState;

        private SessionController _sessionController;

        private EventSubscriptions _subscriptions;

        public MutationResultsController(
            MutationResultsViewModel viewModel,
            IFactory<MutantsCreationController> mutantsCreationFactory,
            IFactory<SessionController> sessionControllerFactory,
            IFactory<ResultsSavingController> resultsSavingFactory,
            
            MutantDetailsController mutantDetailsController,
            CommonServices commonServices)
        {
            _viewModel = viewModel;
            _mutantsCreationFactory = mutantsCreationFactory;
            _sessionControllerFactory = sessionControllerFactory;
            _resultsSavingFactory = resultsSavingFactory;
         //   _mutantsContainer = mutantsContainer;
         //   _testsContainer = testsContainer;
            _mutantDetailsController = mutantDetailsController;
            _commonServices = commonServices;


            _viewModel.CommandSaveResults = new BasicCommand(SaveResults, () => 
                _viewModel.OperationsState == OperationsState.Finished)
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);


            _viewModel.CommandCreateNewMutants = new BasicCommand(RunMutationSession,
                () => _viewModel.OperationsState.IsIn(OperationsState.None, OperationsState.Finished, OperationsState.Error))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandPause = new BasicCommand(PauseOperations, 
                () => _viewModel.OperationsState.IsIn(OperationsState.Testing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandStop = new BasicCommand(StopOperations,
                () => _viewModel.OperationsState.IsIn(
                    OperationsState.Testing, OperationsState.TestingPaused, OperationsState.Pausing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandContinue = new BasicCommand(ResumeOperations,
                () => _viewModel.OperationsState == OperationsState.TestingPaused)
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.Operators = new BetterObservableCollection<ExecutedOperator>();


            _viewModel.RegisterPropertyChanged(vm => vm.SelectedMutationTreeItem).OfType<Mutant>()
                .Subscribe(mutant => _mutantDetailsController.LoadDetails(mutant, _currentSession.OriginalAssemblies));

            _viewModel.MutantDetailsViewModel = _mutantDetailsController.ViewModel;

        }

   
        private void SetState(OperationsState state)
        {
            _commonServices.Threading.InvokeOnGui(() =>
            {
                _viewModel.OperationsState = state;
                _viewModel.OperationsStateDescription = Functional.ValuedSwitch<OperationsState, string>(state)
                    .Case(OperationsState.None, "")
                    .Case(OperationsState.TestingPaused, "Paused")
                    .Case(OperationsState.Finished, "Finished")
                    .Case(OperationsState.PreCheck, "Running pre-check...")
                    .Case(OperationsState.Mutating, "Creating mutants...")
                    .Case(OperationsState.Pausing, "Pausing...")
                    .Case(OperationsState.Stopping, "Stopping...")
                    .Case(OperationsState.Error, "Error occurred.")
                    .Case(OperationsState.Testing, () => "Running tests... ({0}/{1})"
                        .Formatted(_currentSession.TestedMutants.Count,
                            _currentSession.MutantsToTest.Count + _currentSession.TestedMutants.Count))
                    .GetResult();
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changelessMutant"></param>
        /// <returns>true if session can continue</returns>
        private bool ProcessPrecheckMutant(Mutant changelessMutant )
        {

            if (changelessMutant.State == MutantResultState.Error)
            {
                if (changelessMutant.TestSession.Exception is AssemblyVerificationException)
                {
                    _commonServices.Logging.ShowWarning(
                        UserMessages.ErrorPretest_VerificationFailure(changelessMutant.TestSession.Exception.Message), _log);

                    _currentSession.Options.IsMutantVerificationEnabled = false;

                }
                else
                {
                    _commonServices.Logging.ShowError(UserMessages.ErrorPretest_UnknownError(
                        changelessMutant.TestSession.Exception.ToString()), _log);

                    return false;
                }
            }
            else if (changelessMutant.State == MutantResultState.Killed)
            {
                var test = changelessMutant.TestSession.TestMap.Values.FirstOrDefault(t =>
                    t.State == TestNodeState.Failure);
                if (test != null)
                {
                    _commonServices.Logging.ShowError(UserMessages.ErrorPretest_TestsFailed(
                        test.Name, test.Message), _log);
                }
                else
                {
                    var testInconcl = changelessMutant.TestSession.TestMap.Values.First(t =>
                        t.State == TestNodeState.Inconclusive);

                    _commonServices.Logging.ShowError(UserMessages.ErrorPretest_TestsFailed(
                        testInconcl.Name, "Test was inconclusive."), _log);
                }
                return false;
            }
            return true;
        }

        class EventSubscriptions
        {
              public Action OnStartingPreCheck{get;set;} 
              public Action<int> OnMutationsStarting{get;set;} 
              public Action OnMutationProgress{get;set;} 
              public Action OnMutationFinished{get;set;} 

              public Action<int> OnTestingStarting{get;set;} 
              public Action<TestingProgressEventArgs> OnTestingProgress{get;set;} 

              public Action OnSessionPaused{get;set;} 
              public Action OnFinished{get;set;} 
              public Action OnStopping{get;set;} 
              public Action OnFinishedWithError{get;set;}  


            public void Subscribe(SessionController _c)
            {
                _c.OnStartingPreCheck += OnStartingPreCheck;
                _c.OnMutationsStarting += OnMutationsStarting;
                _c.OnMutationProgress += OnMutationProgress;
                _c.OnMutationFinished += OnMutationFinished;

                _c.OnTestingStarting += OnTestingStarting;
                _c.OnTestingProgress += OnTestingProgress;

                _c.OnSessionPaused += OnSessionPaused;
                _c.OnFinished += OnFinished;
                _c.OnStopping += OnStopping;
                _c.OnFinishedWithError += OnFinishedWithError;
            }

        }
        public void RunMutationSession()
        {


            var mutantsCreationController = _mutantsCreationFactory.Create().Run();
  
            if (mutantsCreationController.HasResults)
            {

                MutationSessionChoices choices = mutantsCreationController.Result;

                _sessionController = _sessionControllerFactory.Create();

                _sessionController.Initialize();

                _subscriptions = new EventSubscriptions
                {
                    OnStartingPreCheck = () => SetState(OperationsState.PreCheck),
                    OnMutationsStarting = count =>
                        {
                            SetState(OperationsState.Mutating);
                            _viewModel.InitTestingProgress(count);

                        },
                };

                _sessionController.OnStartingPreCheck += () => SetState(OperationsState.PreCheck);
                _sessionController.On += () => SetState(OperationsState.PreCheck);


     

            }

          
        

            ;

            _commonServices.Threading.ScheduleAsync(() =>
            {
                _currentSession  = _mutantsContainer.PrepareSession(choices);
                Mutant changelessMutant = _mutantsContainer.CreateChangelessMutant(_currentSession);

                _currentSession.TestEnvironment = _testsContainer.InitTestEnvironment(_currentSession);
                _testsContainer.RunTestsForMutant(_currentSession,_currentSession.TestEnvironment, changelessMutant);

                return ProcessPrecheckMutant(changelessMutant);
            },
            canContinue =>
            {
                if (canContinue)
                {
                    CreateMutants();
                }
                else
                {
                    FinishWithError();
                }
            },
            onException: FinishWithError);
            
        }




        public void CreateMutants()
        {
            SetState(OperationsState.Mutating);


            _viewModel.InitTestingProgress(_currentSession.SelectedOperators.Count);
 
            _commonServices.Threading.ScheduleAsync(() =>
            {
                _mutantsContainer.GenerateMutantsForOperators(_currentSession, () => _viewModel.UpdateTestingProgress());
            },
            () =>
            {
                _viewModel.Operators.ReplaceRange(_currentSession.MutantsGroupedByOperators);
                RunTests();

            }, onException: FinishWithError);
            
        }


        public void RunTests()
        {
            var allMutants = _currentSession.MutantsGroupedByOperators.SelectMany(op => op.Mutants);
            _currentSession.MutantsToTest = new Queue<Mutant>(allMutants);
            _viewModel.InitTestingProgress(_currentSession.MutantsToTest.Count);
            
            _commonServices.Threading.ScheduleAsync(() =>
            {
                _currentSession.TestedMutants = new List<Mutant>();
                RunTestsInternal();

            }, onException: FinishWithError);
            
        }

        private void RunTestsInternal()
        {

            while (_currentSession.MutantsToTest.Count != 0 && _requestedHaltState == null)
            {
                SetState(OperationsState.Testing);

                Mutant mutant = _currentSession.MutantsToTest.Dequeue();
                _testsContainer.RunTestsForMutant(_currentSession,_currentSession.TestEnvironment, mutant);
                _currentSession.TestedMutants.Add(mutant);
                _viewModel.UpdateTestingProgress();

                int mutantsKilled = _currentSession.TestedMutants.Count(m => m.State == MutantResultState.Killed);
              //  int mutantsLive = _currentSession.TestedMutants.Count(m => m.State == MutantResultState.Live);
 
                _currentSession.MutationScore = ((double)mutantsKilled) / _currentSession.TestedMutants.Count;
                _viewModel.MutantsRatio = string.Format("Mutants killed: {0}/{1}", mutantsKilled, _currentSession.TestedMutants.Count);
                _viewModel.MutationScore = string.Format("Mutation score: {0}", _currentSession.MutationScore);

            }
            if (_requestedHaltState != null)
            {

                Switch.On(_requestedHaltState)
                    .Case(RequestedHaltState.Pause, () => SetState( OperationsState.TestingPaused))
                    .Case(RequestedHaltState.Stop, () =>
                    {
                        Finish();
                    })
                    .Do();
                _requestedHaltState = null;
            }
            else
            {
                Finish();
            }

        }

        private void Finish()
        {
            
            _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);
            SetState(OperationsState.Finished);
        }
        private void FinishWithError()
        {
            _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);
            _viewModel.TestingProgress = 0;
            SetState(OperationsState.Error);
        }
        public void PauseOperations()
        {
            _requestedHaltState = RequestedHaltState.Pause;
            SetState(OperationsState.Pausing);

        }
        public void ResumeOperations()
        {
            _commonServices.Threading.ScheduleAsync(() =>
            {
                RunTestsInternal();
            }, onException: FinishWithError);
        }

        public void StopOperations()
        {
            if (_viewModel.OperationsState == OperationsState.TestingPaused)
            {
                Finish();
            }
            else
            {
                _requestedHaltState = RequestedHaltState.Stop;
                SetState(OperationsState.Stopping);
                _testsContainer.CancelTestRun();


            }
            
        }
        public void SaveResults()
        {
            var resultsSavingController = _resultsSavingFactory.Create();


            resultsSavingController.Run(_currentSession);

        }

        public void Stop()
        {

        }

        public void Initialize()
        {
            _viewModel.IsVisible = true;
        }

        public void Deactivate()
        {
            Stop();
            Clean();
            _viewModel.IsVisible = false;
        }

        private void Clean()
        {
            _viewModel.Operators.Clear();
        }

        public MutationResultsViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }
    }
}