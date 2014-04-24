namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Controllers;
    using Exceptions;
    using Infrastructure;
    using log4net;
    using Mutations.MutantsTree;
    using StoringMutants;
    using Tests;
    using Tests.Services;
    using Tests.TestsTree;
    using UsefulTools.ExtensionMethods;

    public class TestingMutant
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly TestsContainer _testsContainer;
        private readonly MutationSessionChoices _choices;
        private readonly NUnitXmlTestService _nunitService;
        private readonly ISubject<SessionEventArgs> _sessionEventsSubject;
        
        private readonly Mutant _mutant;
        private StoredMutantInfo _storedMutantInfo;
        private ICollection<NUnitTester> _nUnitTesters;
        private DateTime _sessionStartTime;


        public TestingMutant(
            SessionController sessionController,
            TestsContainer testsContainer,
            MutationSessionChoices choices,
            NUnitXmlTestService nunitService,
            ISubject<SessionEventArgs> sessionEventsSubject,
            Mutant mutant)
        {
            _testsContainer = testsContainer;
            _choices = choices;
            _nunitService = nunitService;
            _sessionEventsSubject = sessionEventsSubject;
            _mutant = mutant;
            _sessionStartTime = sessionController.SessionStartTime;
        }
        public void Cancel()
        {
        }


        public async Task<MutantResultState> RunAsync()
        {
            _mutant.State = MutantResultState.Creating;
            var sw = new Stopwatch();
            sw.Start();
            _storedMutantInfo = await _testsContainer.StoreMutant(_mutant);
            _sessionEventsSubject.OnNext(new MutantStoredEventArgs(_storedMutantInfo));
            if (_choices.MutantsCreationOptions.IsMutantVerificationEnabled)
            {
                bool verResult = _testsContainer.VerifyMutant(_storedMutantInfo, _mutant);
                _sessionEventsSubject.OnNext(new MutantVerifiedEvent(_mutant, verResult));
            }
            sw.Stop();
            _mutant.CreationTimeMilis = sw.ElapsedMilliseconds;
            //                    CodeWithDifference diff = _codeDifferenceCreator.CreateDifferenceListing(
            //                        CodeLanguage.IL, mutant);
            //
            //                    if (diff.LineChanges.Count == 0)
            //                    {
            //                        mutant.IsEquivalent = true;
            //                    }
            if (!_mutant.IsEquivalent) //todo: somewhat non-threadsafe, but valid
            {
                await RunTestsForMutant(_choices.MutantsTestingOptions, _storedMutantInfo);
                _storedMutantInfo.Dispose();

                return _mutant.State;
            }
                //_sessionEventsSubject.OnNext(new MutantTestedEvent(mutant.State));
            else
            {
                return (MutantResultState.Untested);
            }
        }

        public Task RunTestsForMutant(MutantsTestingOptions options, StoredMutantInfo storedMutantInfo)
        {
            _mutant.State = MutantResultState.Tested;

            var sw = new Stopwatch();
            sw.Start();

            _log.Info("Loading tests for mutant " + _mutant.Id);

           

            List<TestsRunContext> contexts = CreateTestContexts(storedMutantInfo.AssembliesPaths,
                _choices.TestAssemblies).ToList();

            _log.Info("Running tests for mutant " + _mutant.Id);

           
            _nUnitTesters = contexts.Select(_nunitService.SpawnTester).ToList();

            IDisposable timoutDisposable =
              Observable.Timer(TimeSpan.FromSeconds(options.TestingTimeoutSeconds))
              .Subscribe(e => CancelTestRun());

            var task = Task.WhenAll(_nUnitTesters.Select(t => t.RunTests()));
            return task.ContinueWith(t =>
            {
                timoutDisposable.Dispose();
                if (t.Exception == null)
                {
                    _log.Debug("Finished waiting for tests. ");
                    _mutant.TestRunContexts = contexts;

                    ResolveMutantState(contexts.Select( c => c.TestResults));

                    _mutant.MutantTestSession.IsComplete = true;
                }
            }).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    SetError(t.Exception.InnerException);
                }
                sw.Stop();
                _mutant.MutantTestSession.TestingTimeMiliseconds = sw.ElapsedMilliseconds;
                _mutant.MutantTestSession.TestingEndRelative = DateTime.Now - _sessionStartTime;
            });

           
        }


        private void CancelTestRun()
        {
            foreach (var nUnitTester in _nUnitTesters)
            {
                nUnitTester.CancelRun();
            }
        }


        private IEnumerable<TestsRunContext> CreateTestContexts(
            List<string> mutatedPaths,
            IList<TestNodeAssembly> testAssemblies)
        {


            foreach (var testNodeAssembly in testAssemblies)
            {
                //todo: get rid of this ungly thing
                var mutatedPath = mutatedPaths.Single(p => Path.GetFileName(p) ==
                    Path.GetFileName(testNodeAssembly.AssemblyPath));

                var originalContext = testNodeAssembly.TestsLoadContext;
                var context = new TestsRunContext();
                context.SelectedTests = originalContext.SelectedTests;
                context.AssemblyPath = mutatedPath;

                yield return context;
            }
        }

        private void SetError(Exception e)
        {
            _mutant.MutantTestSession.ErrorDescription = "Error ocurred";
            _mutant.MutantTestSession.ErrorMessage = e.Message;
            _mutant.MutantTestSession.Exception = e;
            _mutant.State = MutantResultState.Error;
            _log.Error("Set mutant " + _mutant.Id + " error: " + _mutant.State + " message: " + e.Message);
        }


        private void ResolveMutantState(IEnumerable<MutantTestResults> results)
        {
            if(results.Any(r => r.Cancelled))
            {
                _mutant.KilledSubstate = MutantKilledSubstate.Cancelled;
                _mutant.State = MutantResultState.Killed;
                return;
            }

            List<TmpTestNodeMethod> nodeMethods = _mutant.TestRunContexts
                .SelectMany(c => c.TestResults.ResultMethods).ToList();

            _mutant.NumberOfFailedTests = nodeMethods
                          .Count(t => t.State.IsIn(TestNodeState.Failure, TestNodeState.Inconclusive));


            if (nodeMethods.Any(t => t.State == TestNodeState.Inconclusive))
            {

                _mutant.KilledSubstate = MutantKilledSubstate.Inconclusive;
                _mutant.State = MutantResultState.Killed;
            }

            else if (nodeMethods.Any(t => t.State == TestNodeState.Failure))
            {

                _mutant.KilledSubstate = MutantKilledSubstate.Normal;
                _mutant.State = MutantResultState.Killed;
            }
            else if (nodeMethods.All(t => t.State == TestNodeState.Success))
            {
                _mutant.State = MutantResultState.Live;
            }
            else
            {
                throw new InvalidOperationException("Unknown state");
            }
            _log.Info("Resolved mutant" + _mutant.Id + " state: " + _mutant.State + " sub: " + _mutant.KilledSubstate);
        }

    }
}