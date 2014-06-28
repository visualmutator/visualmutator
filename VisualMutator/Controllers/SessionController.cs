namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
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
    using UsefulTools.Wpf;
    using Wintellect.PowerCollections;

    #endregion


    public class SessionController
    {
        private readonly IMutantsContainer _mutantsContainer;

        private readonly IDispatcherExecute _dispatcher;
        private readonly CommonServices _svc;
        private readonly MutantDetailsController _mutantDetailsController;

        private readonly ITestsContainer _testsContainer;
       
        private readonly IFactory<ResultsSavingController> _resultsSavingFactory;
        private readonly IFactory<TestingProcess> _testingProcessFactory;
        private readonly IRootFactory<TestingMutant> _testingMutantFactory;
        private readonly MutationSessionChoices _choices;


        private MutationTestingSession _currentSession;


        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private RequestedHaltState? _requestedHaltState;

        private readonly Subject<SessionEventArgs> _sessionEventsSubject;

        private SessionState _sessionState;


        private TestingProcessExtensionOptions _testingProcessExtensionOptions;
        private readonly List<IDisposable> _subscriptions;
        private TestingProcess _testingProcess;

        public SessionController(
            IDispatcherExecute dispatcher,
            CommonServices svc,
            MutantDetailsController mutantDetailsController,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer,
            IFactory<ResultsSavingController> resultsSavingFactory,
            IFactory<TestingProcess> testingProcessFactory,
            IRootFactory<TestingMutant> testingMutantFactory,
            MutationSessionChoices choices)
        {
            _dispatcher = dispatcher;
            _svc = svc;
            _mutantDetailsController = mutantDetailsController;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _resultsSavingFactory = resultsSavingFactory;
            _testingProcessFactory = testingProcessFactory;
            _testingMutantFactory = testingMutantFactory;
            _choices = choices;

            _sessionState = SessionState.NotStarted;
            _sessionEventsSubject = new Subject<SessionEventArgs>();
            _subscriptions = new List<IDisposable>();


            var subs1 =_sessionEventsSubject.OfType<MutantStoredEventArgs>().Subscribe(e =>
            {
                _testingProcessExtensionOptions.TestingProcessExtension
                    .OnTestingOfMutantStarting(e.StoredMutantInfo.Directory, e.StoredMutantInfo.AssembliesPaths);
            });
            _subscriptions.Add(subs1);

            


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
            try
            {
                _sessionEventsSubject.OnNext(new MinorSessionUpdateEventArgs(type, progress));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void RaiseMinorStatusUpdate(OperationsState type, ProgressUpdateMode mode)
        {
            try
            {
                _sessionEventsSubject.OnNext(new MinorSessionUpdateEventArgs(type, mode));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnTestingStarting(string directory, Mutant mutant)
        {
        
        }
        public async Task RunCore()
        {
            _mutantDetailsController.Initialize();

            _currentSession = new MutationTestingSession
            {
                Filter = _choices.Filter,
                Choices = _choices,
            };

            _testsContainer.CreateTestSelections(_choices.TestAssemblies);

            if (_choices.TestAssemblies.Select(a => a.TestsLoadContext.SelectedTests.TestIds.Count).Sum() == 0)
            {
                throw new NoTestsSelectedException();
            }

            _log.Info("Initializing test environment...");

            _log.Info("Creating pure mutant for initial checks...");
            AssemblyNode assemblyNode;
            Mutant changelessMutant = _mutantsContainer.CreateEquivalentMutant(out assemblyNode);


            _sessionEventsSubject.OnNext(new MutationFinishedEventArgs(OperationsState.MutationFinished)
            {
                MutantsGrouped = assemblyNode.InList(),
            });

            var verifiEvents = _sessionEventsSubject
                .OfType<MutantVerifiedEvent>()
                .Subscribe(e =>
                {
                    if (e.Mutant == changelessMutant && !e.VerificationResult)
                    {
                        _svc.Logging.ShowWarning(UserMessages.ErrorPretest_VerificationFailure(
                            changelessMutant.MutantTestSession.Exception.Message));

                        _choices.MutantsCreationOptions.IsMutantVerificationEnabled = false;
                    }
                });


            IObjectRoot<TestingMutant> testingMutant = _testingMutantFactory
                .CreateWithParams(_sessionEventsSubject, changelessMutant);

            var result = await testingMutant.Get.RunAsync();

            verifiEvents.Dispose();
            _choices.MutantsTestingOptions.TestingTimeoutSeconds
                = (int)((2 * changelessMutant.MutantTestSession.TestingTimeMiliseconds) / 1000 + 1);

            bool canContinue = CheckForTestingErrors(changelessMutant);
            if (!canContinue)
            {
                throw new TestingErrorsException();
            }

            CreateMutants();
            RunTests();
        }
        public async Task RunMutationSession(IObservable<ControlEvent> controlSource)
        {
            try
            {
                Subscribe(controlSource);
                SessionStartTime = DateTime.Now;
                _sessionState = SessionState.Running;
                RaiseMinorStatusUpdate(OperationsState.PreCheck, ProgressUpdateMode.Indeterminate);
                _testingProcessExtensionOptions = _choices.MutantsTestingOptions.TestingProcessExtensionOptions;
                await RunCore();
            }
            catch (Exception e)
            {
                _log.Error(e);
                FinishWithError();
            }
            //            await Task.Run(async () =>
       //     {
                //                    _svc.Threading.PostOnGui(() =>
                //                    {
                //                        if (_requestedHaltState != null)
                //                        {
                //                            _sessionState = SessionState.NotStarted;
                //                            _requestedHaltState = null;
                //                        }
                //                        else
                //                        {
                //                            bool canContinue = CheckForTestingErrors(changelessMutant);
                //                            if (canContinue)
                //                            {
                //                                CreateMutants(continuation: RunTests);
                //                            }
                //                            else
                //                            {
                //                                FinishWithError();
                //                            }
                //                        }
                //                    });
       //     });
            //
            //onException:
            //FinishWithError
        }

        public DateTime SessionStartTime { get; set; }

        private void Subscribe(IObservable<ControlEvent> controlSource)
        {
            _subscriptions.AddRange(
            new List<IDisposable>
            {
                controlSource.Where(ev => ev.Type == ControlEventType.Resume)
                    .Subscribe(o => ResumeOperations()),
                controlSource.Where(ev => ev.Type == ControlEventType.Stop)
                    .Subscribe(o => StopOperations()),
                controlSource.Where(ev => ev.Type == ControlEventType.Pause)
                    .Subscribe(o => PauseOperations()),
                controlSource.Where(ev => ev.Type == ControlEventType.SaveResults)
                    .Subscribe(o => SaveResults()),
            });
}

        private void Finish()
        {
            _log.Info("Finishing mutation session.");
            _sessionState = SessionState.Finished;
            SessionEndTime = DateTime.Now;

            
            _currentSession.MutationScore = _testingProcess.MutationScore;
            if (_testingProcessExtensionOptions != null)
            {
                _testingProcessExtensionOptions.TestingProcessExtension.OnSessionFinished();
            }
            RaiseMinorStatusUpdate(OperationsState.Finished, 100);
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();
            _sessionEventsSubject.OnCompleted();
        }

        public DateTime SessionEndTime { get; set; }

        private void FinishWithError()
        {
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

        public void CreateMutants()
        {
            var counter = ProgressCounter.Invoking(RaiseMinorStatusUpdate, OperationsState.Mutating);

            var mutantModules = _mutantsContainer.InitMutantsForOperators(counter, _choices.WhiteSource);
            _currentSession.MutantsGrouped = mutantModules;

            _sessionEventsSubject.OnNext(new MutationFinishedEventArgs(OperationsState.MutationFinished)
            {
                MutantsGrouped = _currentSession.MutantsGrouped,
            });
        }

        public void RunTests()
        {
            var allMutants = _currentSession.MutantsGrouped.Cast<CheckedNode>()
                .SelectManyRecursive(m => m.Children, leafsOnly: true).OfType<Mutant>().ToList();

            _testingProcess = _testingProcessFactory.CreateWithParams(_sessionEventsSubject, allMutants);

            foreach (var allMutant in allMutants)
            {
                var subscription = allMutant
                    .WhenPropertyChanged(m => m.IsEquivalent)
                    .Subscribe(equivalent => _testingProcess.MarkedAsEqivalent(equivalent));
                _subscriptions.Add(subscription);
            }

            new Thread(RunTestsInternal).Start();
        }
        
        private void RunTestsInternal()
        {
            Action endCallback = () => new TaskFactory(_dispatcher.GuiScheduler).StartNew(() =>
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

            _testingProcess.Start(endCallback);
        }
        public void TestWithHighPriority(Mutant mutant)
        {
            _testingProcess.TestWithHighPriority(mutant);
        }

        public void PauseOperations()
        {
            _log.Info("Requesting pause.");
            _requestedHaltState = RequestedHaltState.Pause;
            _testingProcess.Stop();
            RaiseMinorStatusUpdate(OperationsState.Pausing, ProgressUpdateMode.PreserveValue);
        }

        public void ResumeOperations()
        {
            _log.Info("Requesting resume.");
            new Thread(RunTestsInternal).Start();
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
                _testingProcess.Stop();
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

                var testMethods =  changelessMutant.TestRunContexts 
                    .SelectMany(c => c.TestResults.ResultMethods).ToList();
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
        public void CleanDetails()
        {
            _mutantDetailsController.CleanDetails();
        }
        public ResultsSavingController SaveResults()
        {
            var resultsSavingController = _resultsSavingFactory.Create();
            resultsSavingController.Run(_currentSession);
            return resultsSavingController;
        }

        
    }

    public class TestingErrorsException : Exception
    {
    }
}