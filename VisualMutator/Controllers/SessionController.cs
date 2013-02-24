namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;
    using Model.Mutations.MutantsTree;
    using Model.StoringMutants;
    using Model.Verification;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.Custom;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.ViewModels;

    using log4net;

    #endregion


    public class SessionController
    {
        private readonly IMutantsContainer _mutantsContainer;

        private readonly CommonServices _svc;

        private readonly ITestsContainer _testsContainer;

        private readonly IMutantsFileManager _mutantsFileManager;
       

        private readonly XmlResultsGenerator _xmlResultsGenerator;

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

        public SessionController(
            CommonServices svc,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer,
            IMutantsFileManager mutantsFileManager,
            XmlResultsGenerator xmlResultsGenerator)
        {
            _svc = svc;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _mutantsFileManager = mutantsFileManager;
   

            _xmlResultsGenerator = xmlResultsGenerator;
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
        private void RaiseMinorStatusUpdate(OperationsState type, int progress)
        {
            _sessionEventsSubject.OnNext(new MinorSessionUpdateEventArgs(type, progress));
        }
        private void RaiseMinorStatusUpdate(OperationsState type, ProgressUpdateMode mode)
        {
            _sessionEventsSubject.OnNext(new MinorSessionUpdateEventArgs(type, mode));
        }

      
        public void OnlyCreateMutants(MutationSessionChoices choices)
        {
            _sessionState = SessionState.Running;

            RaiseMinorStatusUpdate(OperationsState.PreCheck, ProgressUpdateMode.Indeterminate);

            _svc.Threading.ScheduleAsync(() =>
            {
                _currentSession = _mutantsContainer.PrepareSession(choices);
                Mutant changelessMutant = _mutantsContainer.CreateChangelessMutant(_currentSession);

                _currentSession.TestEnvironment = _testsContainer.InitTestEnvironment(_currentSession);
                StoredMutantInfo storedMutantInfo = _testsContainer.StoreMutant(_currentSession.TestEnvironment, changelessMutant);
               // StoredMutantInfo storedMutantInfo = _mutantsFileManager
              //      .StoreMutant(.DirectoryPath, changelessMutant);

                TryVerifyPreCheckMutantIfAllowed(storedMutantInfo, changelessMutant);

            },
            () =>
            {

                CreateMutants(continuation: SaveMutants);
               
            },
            onException: FinishWithError);
        }


        private void TryVerifyPreCheckMutantIfAllowed(StoredMutantInfo storedMutantInfo, Mutant changelessMutant)
        {
            if (_currentSession.Choices.MutantsCreationOptions.IsMutantVerificationEnabled
                   && !_testsContainer.VerifyMutant(storedMutantInfo, changelessMutant))
            {
                _svc.Logging.ShowWarning(UserMessages.ErrorPretest_VerificationFailure(
                    changelessMutant.MutantTestSession.Exception.Message), _log);

                _currentSession.Choices.MutantsCreationOptions.IsMutantVerificationEnabled = false;
            }
        }


        private void SaveMutants()
        {
            _svc.Threading.ScheduleAsync(() =>
            {
                
                var counter = ProgressCounter.Invoking(RaiseMinorStatusUpdate, OperationsState.SavingMutants);
    
                Action<Mutant, StoredMutantInfo> verify = (mutant, storageInfo) => {};
                if (_currentSession.Choices.MutantsCreationOptions.IsMutantVerificationEnabled)
                {
                    verify = (mutant, storageInfo) => _testsContainer.VerifyMutant(storageInfo, mutant);
                }

                _mutantsFileManager.WriteMutantsToDisk(_currentSession.Choices.MutantsCreationFolderPath,


             //        StoredMutantInfo storedMutantInfo = _testsContainer.StoreMutant(_currentSession.TestEnvironment, changelessMutant);


                 _currentSession.MutantsGroupedByOperators, verify,counter);


                XDocument results = _xmlResultsGenerator.GenerateResults(_currentSession, includeDetailedTestResults: false, 
                    includeCodeDifferenceListings: true);

                string resultsFilePath = Path.Combine(_currentSession.Choices.MutantsCreationFolderPath, "Results.xml");
                using (var writer = _svc.FileSystem.File.CreateText(resultsFilePath))
                {
                    writer.Write(results.ToString());
                }

            },
           () =>
           {

             Finish();

           },
           onException: FinishWithError);
           

        }

        public void OnTestingStarting(string directory, Mutant mutant)
        {

        }

        public void RunMutationSession(MutationSessionChoices choices)
        {
            _sessionState = SessionState.Running;
            
            RaiseMinorStatusUpdate(OperationsState.PreCheck, ProgressUpdateMode.Indeterminate);

            _testingProcessExtensionOptions = choices.MutantsTestingOptions.TestingProcessExtensionOptions;
            _svc.Threading.ScheduleAsync(() =>
            {
                _currentSession = _mutantsContainer.PrepareSession(choices);
                Mutant changelessMutant = _mutantsContainer.CreateChangelessMutant(_currentSession);

                _currentSession.TestEnvironment = _testsContainer.InitTestEnvironment(_currentSession);

                _testingProcessExtensionOptions.TestingProcessExtension.OnSessionStarting(
                    _testingProcessExtensionOptions.Parameter, choices.ProjectPaths.Select(p=>p.Path).ToList());


                var storedMutantInfo =_testsContainer.StoreMutant(_currentSession.TestEnvironment, changelessMutant);
            
                TryVerifyPreCheckMutantIfAllowed(storedMutantInfo, changelessMutant);



                _testingProcessExtensionOptions.TestingProcessExtension
                    .OnTestingOfMutantStarting(_currentSession.TestEnvironment.DirectoryPath, storedMutantInfo.AssembliesPaths);
                _testsContainer.RunTestsForMutant(_currentSession, storedMutantInfo, changelessMutant);
                return changelessMutant;

            },
            changelessMutant =>
            {
                

                if (_requestedHaltState != null)
                {
                    _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);
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

        private void Finish()
        {
            _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);
            _sessionState = SessionState.Finished;
            RaiseMinorStatusUpdate(OperationsState.Finished, 100);
            _testingProcessExtensionOptions.TestingProcessExtension.OnSessionFinished();
            _sessionEventsSubject.OnCompleted();
        }

        private void FinishWithError()
        {
            _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);

            _sessionState = SessionState.Finished;
            RaiseMinorStatusUpdate(OperationsState.Error, 0);
            _testingProcessExtensionOptions.TestingProcessExtension.OnSessionFinished();
            _sessionEventsSubject.OnCompleted();
        }

        public void CreateMutants(Action continuation )
        {
 
            var counter = ProgressCounter.Invoking(RaiseMinorStatusUpdate, OperationsState.Mutating);

            _svc.Threading.ScheduleAsync(
            () =>
            {
                _mutantsContainer.GenerateMutantsForOperators(_currentSession, counter);
            },
            () =>
            {
                _sessionEventsSubject.OnNext(new MutationFinishedEventArgs(OperationsState.MutationFinished)
                {
                    MutantsGroupedByOperators = _currentSession.MutantsGroupedByOperators,
                });

                continuation();
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

                _mutantsContainer.ExecuteMutation(mutant, 
                    _currentSession.StoredSourceAssemblies.Modules,
                    _currentSession.SelectedTypes.ToList(),ProgressCounter.Inactive());
                var storedMutantInfo = _testsContainer.StoreMutant(_currentSession.TestEnvironment, mutant);
          
                if (_currentSession.Choices.MutantsCreationOptions.IsMutantVerificationEnabled)
                {
                    _testsContainer.VerifyMutant(storedMutantInfo, mutant);
                }

                _testingProcessExtensionOptions.TestingProcessExtension
                    .OnTestingOfMutantStarting(_currentSession.TestEnvironment.DirectoryPath, storedMutantInfo.AssembliesPaths);

                
                _testsContainer.RunTestsForMutant(_currentSession, storedMutantInfo, mutant);
                _testedMutants.Add(mutant);
       
                _mutantsKilledCount = _mutantsKilledCount.IncrementedIf(mutant.State == MutantResultState.Killed);

                _currentSession.MutationScore = ((double)_mutantsKilledCount) / _testedMutants.Count;

                raiseTestingProgress();
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
                        changelessMutant.MutantTestSession.Exception.ToString()), _log);

                return false;
                
            }
            else if (changelessMutant.State == MutantResultState.Killed)
            {
                if (changelessMutant.KilledSubstate == MutantKilledSubstate.Cancelled)
                {
                    return _svc.Logging.ShowYesNoQuestion(UserMessages.ErrorPretest_Cancelled());
                }

                var test = changelessMutant.MutantTestSession.TestMap.Values
                    .FirstOrDefault(t => t.State == TestNodeState.Failure);

                string testName = null;
                string testMessage = null;
                if (test != null)
                {
                    testName = test.Name;
                    testMessage = test.Message;
                    
                }
                else
                {
                    var testInconcl = changelessMutant.MutantTestSession.TestMap.Values
                        .First(t =>t.State == TestNodeState.Inconclusive);

                    testName = testInconcl.Name;
                    testMessage = "Test was inconclusive.";
                }

                return _svc.Logging.ShowYesNoQuestion(UserMessages.ErrorPretest_TestsFailed(testName, testMessage));
            }
            return true;
        }

     
    }

    public class MutationFinishedEventArgs : SessionEventArgs
    {
        public MutationFinishedEventArgs(OperationsState eventType)
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
/*
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
*/
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
    }/*
    public class MutationProgressEventArgs : SessionEventArgs
    {
        public MutationProgressEventArgs(OperationsState eventType)
            : base(eventType)
        {
        }

        
    }*/

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