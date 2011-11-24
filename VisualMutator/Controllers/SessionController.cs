namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.ViewModels;

    using log4net;

    using Event = CommonUtilityInfrastructure.Event;
    enum RequestedHaltState
    {
        Pause,
        Stop
    }
    enum SessionState
    {
        NotStarted,
        Paused,
        Running,
        Finished
    }


    public class TestingProgressEventArgs : EventArgs
    {
        public int NumberOfMutantsKilled { get; set; }

        public int NumberOfAllMutantsTested { get; set; }

        public double MutationScore { get; set; }
    }

    public class SessionController
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CommonServices _svc;

        private readonly IMutantsContainer _mutantsContainer;

        private readonly ITestsContainer _testsContainer;

        private MutationTestingSession _currentSession;

     //   private Action<OperationsState> SetState;

        private bool _isInitialized;

        private RequestedHaltState? _requestedHaltState;

        private SessionState _sessionState;

        public SessionController(
            CommonServices svc,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer)
        {
            _svc = svc;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
        }

        public void Initialize(Action<OperationsState> setState)
        {
         //   SetState = setState;

            _sessionState = SessionState.NotStarted;
            _isInitialized = true;
        }



        public event Action OnStartingPreCheck; 
        public event Action<int> OnMutationsStarting; 
        public event Action OnMutationProgress; 
        public event Action OnMutationFinished;

        public event Action<int> OnTestingStarting;
        public event Action<TestingProgressEventArgs> OnTestingProgress; 

        public event Action OnSessionPaused; 
        public event Action OnFinished;
        public event Action OnStopping; 
        public event Action OnFinishedWithError; 

        private void InvokeEvent(Action evnt)
        {
            if (evnt != null)
            {
                evnt();
            }
        }

        public void RunMutationSession(MutationSessionChoices choices)
        {
            Throw.If(!_isInitialized);

             _sessionState = SessionState.Running;

            Event.RaiseIfNotNull(OnStartingPreCheck);
       
          //  SetState(OperationsState.PreCheck);

            _svc.Threading.ScheduleAsync(() =>
            {
                _currentSession = _mutantsContainer.PrepareSession(choices);
                Mutant changelessMutant = _mutantsContainer.CreateChangelessMutant(_currentSession);

                _currentSession.TestEnvironment = _testsContainer.InitTestEnvironment(_currentSession);
                _testsContainer.RunTestsForMutant(_currentSession, _currentSession.TestEnvironment, changelessMutant);

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



        private void Finish()
        {

            _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);
            Event.RaiseIfNotNull(OnFinished);
           
        }
        private void FinishWithError()
        {
            _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);

            Event.RaiseIfNotNull(OnFinishedWithError);
            
        }




        public void CreateMutants()
        {
            Event.RaiseIfNotNull(OnMutationsStarting, _currentSession.SelectedOperators.Count);


         //   _viewModel.InitTestingProgress(_currentSession.SelectedOperators.Count);

            _svc.Threading.ScheduleAsync(() =>
            {
                _mutantsContainer.GenerateMutantsForOperators(_currentSession, OnMutationProgress);
            },
            () =>
            {
                //_viewModel.Operators.ReplaceRange(_currentSession.MutantsGroupedByOperators);
                Event.RaiseIfNotNull(OnMutationFinished);


                RunTests();

            }, onException: FinishWithError);

        }


        public void RunTests()
        {
           
            var allMutants = _currentSession.MutantsGroupedByOperators.SelectMany(op => op.Mutants);
            _currentSession.MutantsToTest = new Queue<Mutant>(allMutants);
           // _viewModel.InitTestingProgress(_currentSession.MutantsToTest.Count);

            

            _svc.Threading.ScheduleAsync(() =>
            {
                _currentSession.TestedMutants = new List<Mutant>();
                RunTestsInternal();

            }, onException: FinishWithError);

        }

        private void RunTestsInternal()
        {

            while (_currentSession.MutantsToTest.Count != 0 && _requestedHaltState == null)
            {
                //SetState(OperationsState.Testing);
                Event.RaiseIfNotNull(OnTestingStarting, _currentSession.MutantsToTest.Count);

                Mutant mutant = _currentSession.MutantsToTest.Dequeue();
                _testsContainer.RunTestsForMutant(_currentSession, _currentSession.TestEnvironment, mutant);
                _currentSession.TestedMutants.Add(mutant);
               // _viewModel.UpdateTestingProgress();
                
                int mutantsKilled = _currentSession.TestedMutants.Count(m => m.State == MutantResultState.Killed);
                //  int mutantsLive = _currentSession.TestedMutants.Count(m => m.State == MutantResultState.Live);
                _currentSession.MutationScore = ((double)mutantsKilled) / _currentSession.TestedMutants.Count;
                Event.RaiseIfNotNull(OnTestingProgress, new TestingProgressEventArgs
                {
                    NumberOfMutantsKilled = mutantsKilled,
                    NumberOfAllMutantsTested = mutantsKilled,
                    MutationScore = _currentSession.MutationScore,
                });
                
               // _viewModel.MutantsRatio = string.Format("Mutants killed: {0}/{1}", mutantsKilled, _currentSession.TestedMutants.Count);
             //   _viewModel.MutationScore = string.Format("Mutation score: {0}", _currentSession.MutationScore);

            }
            if (_requestedHaltState != null)
            {

                Switch.On(_requestedHaltState)
                    .Case(RequestedHaltState.Pause, () =>
                    {
                        _sessionState = SessionState.Paused;
                        Event.RaiseIfNotNull(OnSessionPaused);
                       // SetState(OperationsState.TestingPaused)
                    })
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


        public void PauseOperations()
        {
            _requestedHaltState = RequestedHaltState.Pause;
         //   SetState(OperationsState.Pausing);

        }
        public void ResumeOperations()
        {
            _svc.Threading.ScheduleAsync(() =>
            {
                RunTestsInternal();
            }, onException: FinishWithError);
        }

        public void StopOperations()
        {
            if (_sessionState == SessionState.Paused)
            {
                Finish();
            }
            else
            {
                _requestedHaltState = RequestedHaltState.Stop;
             //   SetState(OperationsState.Stopping);
                _testsContainer.CancelTestRun();

                Event.RaiseIfNotNull(OnStopping);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="changelessMutant"></param>
        /// <returns>true if session can continue</returns>
        private bool ProcessPrecheckMutant(Mutant changelessMutant)
        {

            if (changelessMutant.State == MutantResultState.Error)
            {
                if (changelessMutant.TestSession.Exception is AssemblyVerificationException)
                {
                    _svc.Logging.ShowWarning(
                        UserMessages.ErrorPretest_VerificationFailure(changelessMutant.TestSession.Exception.Message), _log);

                    _currentSession.Options.IsMutantVerificationEnabled = false;

                }
                else
                {
                    _svc.Logging.ShowError(UserMessages.ErrorPretest_UnknownError(
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
                    _svc.Logging.ShowError(UserMessages.ErrorPretest_TestsFailed(
                        test.Name, test.Message), _log);
                }
                else
                {
                    var testInconcl = changelessMutant.TestSession.TestMap.Values.First(t =>
                        t.State == TestNodeState.Inconclusive);

                    _svc.Logging.ShowError(UserMessages.ErrorPretest_TestsFailed(
                        testInconcl.Name, "Test was inconclusive."), _log);
                }
                return false;
            }
            return true;
        }

    

    }
}