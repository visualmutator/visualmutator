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
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.Custom;
    using Model.Tests.TestsTree;
    using Model.Verification;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;

    #endregion


    public class SessionController
    {
        public IFactory<SessionCreationController> MutantsCreationFactory { get; set; }
        public IFactory<OnlyMutantsCreationController> OnlyMutantsCreationFactory { get; set; }
        private readonly IMutantsContainer _mutantsContainer;

        private readonly CommonServices _svc;
        private readonly MutantDetailsController _mutantDetailsController;

        private readonly ITestsContainer _testsContainer;

        private readonly IMutantsFileManager _mutantsFileManager;
        private readonly IMutantsCache _mutantsCache;


        private readonly XmlResultsGenerator _xmlResultsGenerator;
        private readonly ICodeVisualizer _codeVisualizer;
        private readonly SessionCreationController _scc;
        private readonly IFactory<ResultsSavingController> _resultsSavingFactory;
        private readonly ICommonCompilerInfra _commonCompiler;

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
            MutantDetailsController mutantDetailsController,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer,
            IMutantsFileManager mutantsFileManager,
            IMutantsCache mutantsCache,
            XmlResultsGenerator xmlResultsGenerator,
            ICodeVisualizer codeVisualizer,
            SessionCreationController scc,
            IFactory<SessionCreationController> mutantsCreationFactory,
            IFactory<OnlyMutantsCreationController> onlyMutantsCreationFactory,
            IFactory<ResultsSavingController> resultsSavingFactory,
            ICommonCompilerInfra commonCompiler)
        {
            MutantsCreationFactory = mutantsCreationFactory;
            OnlyMutantsCreationFactory = onlyMutantsCreationFactory;
            _svc = svc;
            _mutantDetailsController = mutantDetailsController;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _mutantsFileManager = mutantsFileManager;
            _mutantsCache = mutantsCache;


            _xmlResultsGenerator = xmlResultsGenerator;
            _codeVisualizer = codeVisualizer;
            _scc = scc;
            _resultsSavingFactory = resultsSavingFactory;
            _commonCompiler = commonCompiler;
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

      
        public void OnlyCreateMutants(MutationSessionChoices choices)
        {
            _sessionState = SessionState.Running;

            RaiseMinorStatusUpdate(OperationsState.PreCheck, ProgressUpdateMode.Indeterminate);

            _svc.Threading.ScheduleAsync(() =>
            {
                var allowedTypes = choices.SelectedTypes.GetIdentifiers();

                _currentSession = new MutationTestingSession
                {
                    OriginalAssemblies = choices.Assemblies,
                    SelectedTypes = allowedTypes,
                    Choices = choices,
                };
                _mutantsContainer.Initialize(choices.MutantsCreationOptions, allowedTypes, choices.Assemblies.Select(_ => _.AssemblyPath).ToList());

                ExecutedOperator oper;
                Mutant changelessMutant = _mutantsContainer.CreateEquivalentMutant(out oper);

                _currentSession.TestEnvironment = _testsContainer.InitTestEnvironment(_currentSession);
                StoredMutantInfo storedMutantInfo = _testsContainer.StoreMutant(_currentSession.TestEnvironment, changelessMutant);
               
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
                    changelessMutant.MutantTestSession.Exception.Message));

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

                //        StoredMutantInfo storedMutantInfo = _testsContainer.StoreMutant(_currentSession.TestEnvironment, changelessMutant);

                _mutantsFileManager.WriteMutantsToDisk(_currentSession.Choices.MutantsCreationFolderPath,
                        _currentSession.MutantsGroupedByOperators, verify,counter);


                XDocument results = _xmlResultsGenerator.GenerateResults(_currentSession, includeDetailedTestResults: false, 
                    includeCodeDifferenceListings: true);

                string resultsFilePath = Path.Combine(_currentSession.Choices.MutantsCreationFolderPath, "Results.xml");
                using (var writer = _svc.FileSystem.File.CreateText(resultsFilePath))
                {
                    writer.Write(results.ToString());
                }

            },
           onGui: Finish,
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
                var allowedTypes = choices.SelectedTypes.GetIdentifiers();
                _mutantsContainer.Initialize(choices.MutantsCreationOptions, allowedTypes, choices.Assemblies.Select(_ => _.AssemblyPath).ToList());

                _currentSession = new MutationTestingSession
                {
                    OriginalAssemblies = choices.Assemblies,
                    SelectedTypes = allowedTypes,
                    Choices = choices,
                };


                _currentSession.TestEnvironment = _testsContainer.InitTestEnvironment(_currentSession);
              
                _log.Info("Creating pure mutant for initial checks...");
                ExecutedOperator execOperator;
                Mutant changelessMutant = _mutantsContainer.CreateEquivalentMutant(out execOperator);
                

                _svc.Threading.InvokeOnGui(() =>
                    {
                        _sessionEventsSubject.OnNext(new MutationFinishedEventArgs(OperationsState.MutationFinished)
                        {
                            MutantsGroupedByOperators = execOperator.InList(),
                        });

                    });

                _log.Info("Initializing test environment...");
                

                _testingProcessExtensionOptions.TestingProcessExtension.OnSessionStarting(
                    _testingProcessExtensionOptions.Parameter, choices.ProjectPaths.Select(p=>p.Path).ToList());

                _log.Info("Writing pure mutant to disk...");
                var storedMutantInfo = _testsContainer.StoreMutant(_currentSession.TestEnvironment, changelessMutant);

                _log.Info("Verifying IL code of pure mutant...");

                TryVerifyPreCheckMutantIfAllowed(storedMutantInfo, changelessMutant);

               

                _testingProcessExtensionOptions.TestingProcessExtension
                    .OnTestingOfMutantStarting(_currentSession.TestEnvironment.DirectoryPath, storedMutantInfo.AssembliesPaths);

                _log.Info("Running tests for pure mutant...");
                _testsContainer.RunTestsForMutant(_currentSession.Choices.MutantsTestingOptions, 
                    storedMutantInfo, changelessMutant, choices.SelectedTests);
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
            if (_currentSession != null)
            {
                _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);
            }
            _sessionState = SessionState.Finished;
            RaiseMinorStatusUpdate(OperationsState.Finished, 100);
            _testingProcessExtensionOptions.TestingProcessExtension.OnSessionFinished();
            _sessionEventsSubject.OnCompleted();
        }

        private void FinishWithError()
        {
            if (_currentSession!= null)
            {
                _testsContainer.CleanupTestEnvironment(_currentSession.TestEnvironment);
            }
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
                var executedOperators = _mutantsContainer.InitMutantsForOperators(_currentSession.Choices.SelectedOperators,
                    _currentSession.SelectedTypes, new ModulesProvider(_currentSession.OriginalAssemblies.Select(_ => _.AssemblyDefinition).ToList()), counter);
                _currentSession.MutantsGroupedByOperators = executedOperators;
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
            _mutantsToTest = new Queue<Mutant>(_currentSession.MutantsGroupedByOperators
                .SelectMany(op => op.MutantGroups).SelectMany(g => g.Mutants));
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
                    var storedMutantInfo = _testsContainer.StoreMutant(_currentSession.TestEnvironment, mutant);

                    if (_currentSession.Choices.MutantsCreationOptions.IsMutantVerificationEnabled)
                    {
                        _testsContainer.VerifyMutant(storedMutantInfo, mutant);
                    }

                    _testingProcessExtensionOptions.TestingProcessExtension
                        .OnTestingOfMutantStarting(_currentSession.TestEnvironment.DirectoryPath, storedMutantInfo.AssembliesPaths);


                    _testsContainer.RunTestsForMutant(_currentSession.Choices.MutantsTestingOptions, storedMutantInfo, 
                        mutant, _currentSession.Choices.SelectedTests);

                    _testedMutants.Add(mutant);

                    _mutantsKilledCount = _mutantsKilledCount.IncrementedIf(mutant.State == MutantResultState.Killed);

                    _currentSession.MutationScore = ((double)_mutantsKilledCount) / _testedMutants.Count;

                    raiseTestingProgress();
                }
                catch (Exception e)
                {
                    _log.Error(e);
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


        public void LoadDetails(Mutant mutant)
        {
            _mutantDetailsController.LoadDetails(mutant, Session);
        }

        public void SaveResults()
        {
            var resultsSavingController = _resultsSavingFactory.Create();
            resultsSavingController.Run(Session);
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