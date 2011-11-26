namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.ViewModels;

    using log4net;

    #endregion


    public class SessionController
    {
        private readonly IMutantsContainer _mutantsContainer;

        private readonly CommonServices _svc;

        private readonly ITestsContainer _testsContainer;

        private int _allMutantsCount;

        private MutationTestingSession _currentSession;


        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private int _mutantsKilledCount;

        private Queue<Mutant> _mutantsToTest;

        private RequestedHaltState? _requestedHaltState;

        private Subject<SessionEventArgs> _sessionEventsSubject;

        private SessionState _sessionState;

        private List<Mutant> _testedMutants;

        public SessionController(
            CommonServices svc,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer)
        {
            _svc = svc;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _sessionState = SessionState.NotStarted;

            _sessionEventsSubject = new Subject<SessionEventArgs>();
        }

        public IObservable<SessionEventArgs> SessionEventsObservable
        {
            get
            {
                return _sessionEventsSubject.AsObservable();
            }
        }


        public MutationTestingSession Session
        {
            get
            {
                return _currentSession;
            }
            
        }

        private void RaiseEvent(SessionEventType type)
        {
            _sessionEventsSubject.OnNext(new SessionEventArgs(type));
        }

        public void RunMutationSession(MutationSessionChoices choices)
        {
            _sessionState = SessionState.Running;

            RaiseEvent(SessionEventType.PreCheckStarting);

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
            _sessionState = SessionState.Finished;
            RaiseEvent(SessionEventType.SessionFinished);
            _sessionEventsSubject.OnCompleted();
        }

        private void FinishWithError()
        {
            _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);

            _sessionState = SessionState.Finished;
            RaiseEvent(SessionEventType.SessionFinishedWithError);
            _sessionEventsSubject.OnCompleted();
        }

        public void CreateMutants()
        {
            Action<int> onMutationProgress = (percent) =>
            {
                _sessionEventsSubject.OnNext(new MutationProgressEventArgs(SessionEventType.MutationProgress)
                {
                    PercentCompleted = percent,
                });
            };

            onMutationProgress(0);

            _svc.Threading.ScheduleAsync(
            () =>
            {
                _mutantsContainer.GenerateMutantsForOperators(_currentSession, onMutationProgress);
            },
            () =>
            {
                _sessionEventsSubject.OnNext(new MutationFinishedEventArgs(SessionEventType.MutationFinished)
                {
                    MutantsGroupedByOperators = _currentSession.MutantsGroupedByOperators,
                });
        
                RunTests();
            }, onException: FinishWithError);
        }

        public void RunTests()
        {
            _mutantsToTest = new Queue<Mutant>(_currentSession.MutantsGroupedByOperators.SelectMany(op => op.Mutants));
            _allMutantsCount = _mutantsToTest.Count;
            _testedMutants = new List<Mutant>();
         
            _svc.Threading.ScheduleAsync(RunTestsInternal, onException: FinishWithError);
        }

        private void RunTestsInternal()
        {
            while (_mutantsToTest.Count != 0 && _requestedHaltState == null)
            {

                Action raiseTestingProgress = () =>
                {
                    _sessionEventsSubject.OnNext(new TestingProgressEventArgs(SessionEventType.TestingProgress)
                    {
                        NumberOfAllMutants = _allMutantsCount,
                        NumberOfMutantsKilled = _mutantsKilledCount,
                        NumberOfAllMutantsTested = _testedMutants.Count,
                        MutationScore = _currentSession.MutationScore,
                    });
                };

                raiseTestingProgress();

                Mutant mutant = _mutantsToTest.Dequeue();
                _testsContainer.RunTestsForMutant(_currentSession, _currentSession.TestEnvironment, mutant);
                _testedMutants.Add(mutant);
       
                _mutantsKilledCount = _mutantsKilledCount.IncrementedIf(mutant.State == MutantResultState.Killed);

                _currentSession.MutationScore = ((double)_mutantsKilledCount) / _testedMutants.Count;

                raiseTestingProgress();
            }
            if (_requestedHaltState != null)
            {
                Switch.On(_requestedHaltState)
                    .Case(RequestedHaltState.Pause, () =>
                    {
                        _sessionState = SessionState.Paused;
                        RaiseEvent(SessionEventType.SessionPaused);
                    })
                    .Case(RequestedHaltState.Stop, () => { Finish(); })
                    .ThrowIfNoMatch();
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
        }

        public void ResumeOperations()
        {
            _svc.Threading.ScheduleAsync(() => { RunTestsInternal(); }, onException: FinishWithError);
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
                _testsContainer.CancelTestRun();
                RaiseEvent(SessionEventType.SessionStopping);

            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "changelessMutant"></param>
        /// <returns>true if session can continue</returns>
        private bool ProcessPrecheckMutant(Mutant changelessMutant)
        {
            if (changelessMutant.State == MutantResultState.Error)
            {
                if (changelessMutant.TestSession.Exception is AssemblyVerificationException)
                {
                    _svc.Logging.ShowWarning(UserMessages.ErrorPretest_VerificationFailure(
                        changelessMutant.TestSession.Exception.Message), _log);

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
                var test = changelessMutant.TestSession.TestMap.Values
                    .FirstOrDefault(t => t.State == TestNodeState.Failure);
                if (test != null)
                {
                    _svc.Logging.ShowError(UserMessages.ErrorPretest_TestsFailed(test.Name, test.Message), _log);
                }
                else
                {
                    var testInconcl = changelessMutant.TestSession.TestMap.Values
                        .First(t =>t.State == TestNodeState.Inconclusive);

                    _svc.Logging.ShowError(UserMessages
                        .ErrorPretest_TestsFailed(testInconcl.Name, "Test was inconclusive."), _log);
                }
                return false;
            }
            return true;
        }
    }

    public class MutationFinishedEventArgs : SessionEventArgs
    {
        public MutationFinishedEventArgs(SessionEventType eventType)
            : base(eventType)
        {
        }

        public IList<ExecutedOperator> MutantsGroupedByOperators { get; set; }
    }

    internal enum RequestedHaltState
    {
        Pause,

        Stop
    }

    internal enum SessionState
    {
        NotStarted,

        Paused,

        Running,

        Finished
    }

    public enum SessionEventType
    {
        PreCheckStarting,




        MutationProgress,

        TestingProgress,

        MutationFinished,
        SessionPaused,

        SessionFinished,

        SessionStopping,

        SessionFinishedWithError,
    }

    public class SessionEventArgs : EventArgs
    {
        private SessionEventType _eventType;

        public SessionEventArgs(SessionEventType eventType)
        {
            _eventType = eventType;
        }

        public SessionEventType EventType
        {
            get
            {
                return _eventType;
            }
        }
    }

    public class MutationProgressEventArgs : SessionEventArgs
    {
        public MutationProgressEventArgs(SessionEventType eventType)
            : base(eventType)
        {
        }

        public int PercentCompleted
        {
            get;
            set;
        }
    }

    public class TestingProgressEventArgs : SessionEventArgs
    {
        public TestingProgressEventArgs(SessionEventType eventType)
            : base(eventType)
        {
        }

        public int NumberOfMutantsKilled
        {
            get;
            set;
        }

        public int NumberOfAllMutantsTested
        {
            get;
            set;
        }

        public double MutationScore
        {
            get;
            set;
        }

        public int NumberOfAllMutants
        {
            get;
            set;
        }
    }
}