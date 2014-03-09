namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using Infrastructure;
    using log4net;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Exceptions;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.Custom;
    using Model.Tests.TestsTree;
    using Model.Verification;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;

    #endregion


    public class SessionController
    {
        public IFactory<CreationController> MutantsCreationFactory { get; set; }
        private readonly IMutantsContainer _mutantsContainer;

        private readonly CommonServices _svc;
        private readonly MutantDetailsController _mutantDetailsController;

        private readonly ITestsContainer _testsContainer;
       
        private readonly IMutantsCache _mutantsCache;


        private readonly IFactory<ResultsSavingController> _resultsSavingFactory;
        private readonly ICodeDifferenceCreator _codeDifferenceCreator;
        private readonly MutationSessionChoices _choices;

        private int _allMutantsCount;

        private MutationTestingSession _currentSession;


        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private int _mutantsKilledCount;

        private Queue<Mutant> _mutantsToTest;

        private RequestedHaltState? _requestedHaltState;

        private Subject<SessionEventArgs> _sessionEventsSubject;

        private SessionState _sessionState;

        private List<Mutant> _testedMutants;

        private TestingProcessExtensionOptions _testingProcessExtensionOptions;
        private List<IDisposable> _subscriptions;

        public SessionController(
            CommonServices svc,
            MutantDetailsController mutantDetailsController,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer,
            IMutantsCache mutantsCache,
            IFactory<CreationController> mutantsCreationFactory,
            IFactory<ResultsSavingController> resultsSavingFactory,
            ICodeDifferenceCreator codeDifferenceCreator,
            MutationSessionChoices choices)
        {
            MutantsCreationFactory = mutantsCreationFactory;
            _svc = svc;
            _mutantDetailsController = mutantDetailsController;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _mutantsCache = mutantsCache;


            _resultsSavingFactory = resultsSavingFactory;
            _codeDifferenceCreator = codeDifferenceCreator;
            _choices = choices;
            _sessionState = SessionState.NotStarted;

            _sessionEventsSubject = new Subject<SessionEventArgs>();
            _subscriptions = new List<IDisposable>();
        }

        public IObservable<SessionEventArgs> SessionEventsObservable
        {
            get
            {
                return _sessionEventsSubject.AsObservable();
            }
        }


        public MutantDetailsController MutantDetailsController
        {
            get
            {
                return _mutantDetailsController;
            }
        }

        private void RaiseMinorStatusUpdate(OperationsState type, int progress)
        {
            _sessionEventsSubject.OnNext(new MinorSessionUpdateEventArgs(type, progress));
        }
        private void RaiseMinorStatusUpdate(OperationsState type, ProgressUpdateMode mode)
        {
            _sessionEventsSubject.OnNext(new MinorSessionUpdateEventArgs(type, mode));
        }

      
   

        private void TryVerifyPreCheckMutantIfAllowed(StoredMutantInfo storedMutantInfo, Mutant changelessMutant)
        {
            if (_choices.MutantsCreationOptions.IsMutantVerificationEnabled
                   && !_testsContainer.VerifyMutant(storedMutantInfo, changelessMutant))
            {
                _svc.Logging.ShowWarning(UserMessages.ErrorPretest_VerificationFailure(
                    changelessMutant.MutantTestSession.Exception.Message));

                _choices.MutantsCreationOptions.IsMutantVerificationEnabled = false;
            }
        }


        public void OnTestingStarting(string directory, Mutant mutant)
        {
        
        }

        public void RunMutationSession(IObservable<ControlEvent> controlSource)
        {
            Subscribe(controlSource);

            MutationSessionChoices choices = _choices;
            _sessionState = SessionState.Running;

            RaiseMinorStatusUpdate(OperationsState.PreCheck, ProgressUpdateMode.Indeterminate);

            _testingProcessExtensionOptions = choices.MutantsTestingOptions.TestingProcessExtensionOptions;
            _svc.Threading.ScheduleAsync(() =>
            {
                _mutantsCache.WhiteCache.Initialize(choices.AssembliesPaths);

                _mutantsContainer.Initialize(choices.SelectedOperators, 
                    choices.MutantsCreationOptions, choices.Filter);

                _currentSession = new MutationTestingSession
                {
                    Filter = choices.Filter,
                    Choices = choices,
                };

                _currentSession.ProjectFilesClone = _testsContainer.InitTestEnvironment();
                _testsContainer.CreateTestSelections(choices.TestAssemblies);

                if (choices.TestAssemblies.Select(a => a.TestsLoadContext.SelectedTests.TestIds.Count).Sum() == 0)
                {
                    throw new NoTestsSelectedException();
                }

                _mutantDetailsController.Initialize(choices.Assemblies);
                _log.Info("Creating pure mutant for initial checks...");
                AssemblyNode assemblyNode;
                Mutant changelessMutant = _mutantsContainer.CreateEquivalentMutant(out assemblyNode);
                

                _svc.Threading.InvokeOnGui(() =>
                    {
                        _sessionEventsSubject.OnNext(new MutationFinishedEventArgs(OperationsState.MutationFinished)
                        {
                            MutantsGrouped = assemblyNode.InList(),
                        });

                    });

                _log.Info("Initializing test environment...");
                

                _testingProcessExtensionOptions.TestingProcessExtension.OnSessionStarting(
                    _testingProcessExtensionOptions.Parameter, choices.ProjectPaths.Select(p=>p.Path).ToList());

                _log.Info("Writing pure mutant to disk...");
                var storedMutantInfo = _testsContainer.StoreMutant(_currentSession.ProjectFilesClone, changelessMutant);

                _log.Info("Verifying IL code of pure mutant...");

                TryVerifyPreCheckMutantIfAllowed(storedMutantInfo, changelessMutant);

                

                _testingProcessExtensionOptions.TestingProcessExtension
                    .OnTestingOfMutantStarting(_currentSession.ProjectFilesClone.ParentPath.Path, storedMutantInfo.AssembliesPaths);

                _log.Info("Running tests for pure mutant...");
                _testsContainer.RunTestsForMutant(_choices.MutantsTestingOptions, 
                    storedMutantInfo, changelessMutant);
                return changelessMutant;

            },
            changelessMutant =>
            {
                

                if (_requestedHaltState != null)
                {
                    _currentSession.ProjectFilesClone.Dispose();
                    _sessionState = SessionState.NotStarted;
                    _requestedHaltState = null;
                    
                }
                else
                {
                    bool canContinue  = CheckForTestingErrors(changelessMutant);
                    if (canContinue)
                    {
                        CreateMutants(continuation: RunTests);
                    }
                    else
                    {
                        FinishWithError();
                    }
                }
                
            },
            onException: FinishWithError);
        }

        private void Subscribe(IObservable<ControlEvent> controlSource)
        {
            _subscriptions.Add(
                controlSource.Where(ev => ev.Type == ControlEventType.Resume)
                .Subscribe(o => ResumeOperations()));
            _subscriptions.Add(
                controlSource.Where(ev => ev.Type == ControlEventType.Pause)
                .Subscribe(o => PauseOperations()));
            _subscriptions.Add(
                controlSource.Where(ev => ev.Type == ControlEventType.Stop)
                .Subscribe(o => StopOperations()));
            _subscriptions.Add(
                controlSource.Where(ev => ev.Type == ControlEventType.SaveResults)
                .Subscribe(o => SaveResults()));
        }

        private void Finish()
        {
            if (_currentSession != null)
            {
                _currentSession.ProjectFilesClone.Dispose();
            }
            _sessionState = SessionState.Finished;
            RaiseMinorStatusUpdate(OperationsState.Finished, 100);
            if (_testingProcessExtensionOptions != null)
            {
                _testingProcessExtensionOptions.TestingProcessExtension.OnSessionFinished();
            }

            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();
            _sessionEventsSubject.OnCompleted();
        }

        private void FinishWithError()
        {
            if (_currentSession!= null)
            {
                _currentSession.ProjectFilesClone.Dispose();
            }
            _sessionState = SessionState.Finished;
            RaiseMinorStatusUpdate(OperationsState.Error, 0);
            if (_testingProcessExtensionOptions != null)
            {
                _testingProcessExtensionOptions.TestingProcessExtension.OnSessionFinished();
            }
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();
            _sessionEventsSubject.OnCompleted();
        }

        public void CreateMutants(Action continuation )
        {
 
            var counter = ProgressCounter.Invoking(RaiseMinorStatusUpdate, OperationsState.Mutating);

            _svc.Threading.ScheduleAsync(
            () =>
            {

                var mutantModules = _mutantsContainer.InitMutantsForOperators(counter);
                _currentSession.MutantsGrouped = mutantModules;
            },
            () =>
            {
                _sessionEventsSubject.OnNext(new MutationFinishedEventArgs(OperationsState.MutationFinished)
                {
                    MutantsGrouped = _currentSession.MutantsGrouped,
                });

                continuation();
            }, onException: FinishWithError);
        }

        public void RunTests()
        {
            _mutantsToTest = new Queue<Mutant>(_currentSession.MutantsGrouped.Cast<CheckedNode>()
                .SelectManyRecursive(m => m.Children, leafsOnly:true).OfType<Mutant>());
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
                    _sessionEventsSubject.OnNext(new TestingProgressEventArgs(OperationsState.Testing)
                    {
                        NumberOfAllMutants = _allMutantsCount,
                        NumberOfMutantsKilled = _mutantsKilledCount,
                        NumberOfAllMutantsTested = _testedMutants.Count,
                        MutationScore = _currentSession.MutationScore,
                    });
                };

                raiseTestingProgress();

                Mutant mutant = _mutantsToTest.Dequeue();
                mutant.State = MutantResultState.Creating;

                try
                {
                    //todo:
                    _currentSession.ProjectFilesClone = _testsContainer.InitTestEnvironment();
                    var storedMutantInfo = _testsContainer.StoreMutant(_currentSession.ProjectFilesClone, mutant);

                    if (_choices.MutantsCreationOptions.IsMutantVerificationEnabled)
                    {
                        _testsContainer.VerifyMutant(storedMutantInfo, mutant);
                    }

                    _testingProcessExtensionOptions.TestingProcessExtension
                        .OnTestingOfMutantStarting(_currentSession.ProjectFilesClone.ParentPath.Path, storedMutantInfo.AssembliesPaths);


                    CodeWithDifference diff = _codeDifferenceCreator.CreateDifferenceListing(
                        CodeLanguage.IL, mutant);

                    if (diff.LineChanges.Count == 0)
                    {   
                        mutant.IsEquivalent = true;
                    }
                    else
                    {
                        _testsContainer.RunTestsForMutant(_choices.MutantsTestingOptions,
                           storedMutantInfo, mutant);

                        _testedMutants.Add(mutant);

                        _mutantsKilledCount = _mutantsKilledCount.IncrementedIf(mutant.State == MutantResultState.Killed);

                    }
                    _currentSession.MutationScore = ((double)_mutantsKilledCount) / _testedMutants.Count;

                    raiseTestingProgress();
                }
                catch (Exception e)
                {
                    _log.Error(e);
                    mutant.MutantTestSession.ErrorDescription = e.Message;
                    mutant.MutantTestSession.ErrorMessage = e.Message;
                    mutant.MutantTestSession.Exception = e;
                    mutant.State = MutantResultState.Error;
                }
             
            }
            
            _svc.Threading.InvokeOnGui(()=>
            {
                if (_requestedHaltState != null)
                {
                    Switch.On(_requestedHaltState)
                    .Case(RequestedHaltState.Pause, () =>
                    {
                        _sessionState = SessionState.Paused;
                        RaiseMinorStatusUpdate(OperationsState.TestingPaused, ProgressUpdateMode.PreserveValue);
                    })
                    .Case(RequestedHaltState.Stop, Finish)
                    .ThrowIfNoMatch();
                    _requestedHaltState = null;
                }
                else
                {
                    Finish();
                }
            });
                
            
        }

        public void PauseOperations()
        {
            _requestedHaltState = RequestedHaltState.Pause;
            RaiseMinorStatusUpdate(OperationsState.Pausing, ProgressUpdateMode.PreserveValue);
        }

        public void ResumeOperations()
        {
            _svc.Threading.ScheduleAsync(RunTestsInternal, onException: FinishWithError);
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
                _testsContainer.CancelAllTesting();
                RaiseMinorStatusUpdate(OperationsState.Stopping, ProgressUpdateMode.PreserveValue);

            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "changelessMutant"></param>
        /// <returns>true if session can continue</returns>
        private bool CheckForTestingErrors(Mutant changelessMutant)
        {
            if (changelessMutant.State == MutantResultState.Error && 
                !(changelessMutant.MutantTestSession.Exception is AssemblyVerificationException))
            {
                
                _svc.Logging.ShowError(UserMessages.ErrorPretest_UnknownError(
                        changelessMutant.MutantTestSession.Exception.ToString()));

                return false;
                
            }
            else if (changelessMutant.State == MutantResultState.Killed)
            {
                if (changelessMutant.KilledSubstate == MutantKilledSubstate.Cancelled)
                {
                    return _svc.Logging.ShowYesNoQuestion(UserMessages.ErrorPretest_Cancelled());
                }

                var testMethods = changelessMutant.TestResults.SelectMany(r => r.ResultMethods).ToList();

              //  MutantTestSession.TestsByAssembly.Values
               //     .SelectMany(v => v.TestMap.Values)
                var test = testMethods.FirstOrDefault(t => t.State == TestNodeState.Failure);

                string testName = null;
                string testMessage = null;
                if (test != null)
                {
                    testName = test.Name;
                    testMessage = test.Message;
                    
                }
                else
                {
                    var testInconcl = testMethods
                        .First(t =>t.State == TestNodeState.Inconclusive);

                    testName = testInconcl.Name;
                    testMessage = "Test was inconclusive.";
                }

                return _svc.Logging.ShowYesNoQuestion(UserMessages.ErrorPretest_TestsFailed(testName, testMessage));
            }
            return true;
        }


        public void LoadDetails(Mutant mutant)
        {
            _mutantDetailsController.LoadDetails(mutant);
        }

        public void SaveResults()
        {
            var resultsSavingController = _resultsSavingFactory.Create();
            resultsSavingController.Run(_currentSession);
        }
    }

    public class MutationFinishedEventArgs : SessionEventArgs
    {
        public MutationFinishedEventArgs(OperationsState eventType)
            : base(eventType)
        {
        }

        public IList<AssemblyNode> MutantsGrouped { get; set; }
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

    public class SessionEventArgs : EventArgs
    {
        private OperationsState _eventType;

        public SessionEventArgs(OperationsState eventType)
        {
            _eventType = eventType;
        }

        public OperationsState EventType
        {
            get
            {
                return _eventType;
            }
        }
    }

    public enum ProgressUpdateMode
    {
        SetValue,
        Indeterminate,
        PreserveValue
    }
    public class MinorSessionUpdateEventArgs : SessionEventArgs
    {

        public MinorSessionUpdateEventArgs(OperationsState eventType, int progress = 0)
            : base(eventType)
        {
            _progressUpdateMode = ProgressUpdateMode.SetValue;
            _percentCompleted = progress;
        }
        public MinorSessionUpdateEventArgs(OperationsState eventType, ProgressUpdateMode progressUpdateMode)
            : base(eventType)
        {
            _progressUpdateMode = progressUpdateMode;
            _percentCompleted = 0;
        }

        private readonly ProgressUpdateMode _progressUpdateMode;

        private int _percentCompleted   ;

        public ProgressUpdateMode ProgressUpdateMode
        {
            get
            {
                return _progressUpdateMode;
            }
        }

        public int PercentCompleted
        {
            get
            {
                return _percentCompleted;
            }
            
        }
    }
    
    public class TestingProgressEventArgs : SessionEventArgs
    {
        public TestingProgressEventArgs(OperationsState eventType)
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