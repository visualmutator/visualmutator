namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Controllers;
    using Exceptions;
    using log4net;
    using Mutations.MutantsTree;
    using StoringMutants;
    using Tests;
    using Tests.Services;
    using Tests.TestsTree;
    using UsefulTools.ExtensionMethods;

    public class TestingMutant
    {
        private readonly TestsContainer _testsContainer;
        private readonly MutationSessionChoices _choices;
        private readonly ISubject<SessionEventArgs> _sessionEventsSubject;
        private readonly Mutant _mutant;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Mutant mutant;
        private IDisposable _timoutDisposable;
        private bool _cancelRequested;
        private StoredMutantInfo _storedMutantInfo;


        public TestingMutant(
            TestsContainer testsContainer,
            MutationSessionChoices choices,
            ISubject<SessionEventArgs> sessionEventsSubject,
            Mutant mutant)
        {
            _testsContainer = testsContainer;
            _choices = choices;
            _sessionEventsSubject = sessionEventsSubject;
            this.mutant = mutant;



        }
        public void Cancel()
        {
            _cancelRequested = true;
        }


        public Task RunAsync()
        {
            _storedMutantInfo = _testsContainer.StoreMutant(mutant);
            _sessionEventsSubject.OnNext(new MutantStoredEventArgs(_storedMutantInfo));
            if (_choices.MutantsCreationOptions.IsMutantVerificationEnabled)
            {
                bool verResult = _testsContainer.VerifyMutant(_storedMutantInfo, mutant);
                _sessionEventsSubject.OnNext(new MutantVerifiedEvent(mutant, verResult));
            }
            

            //                    CodeWithDifference diff = _codeDifferenceCreator.CreateDifferenceListing(
            //                        CodeLanguage.IL, mutant);
            //
            //                    if (diff.LineChanges.Count == 0)
            //                    {
            //                        mutant.IsEquivalent = true;
            //                    }
            if (!mutant.IsEquivalent) //todo: somewhat non-threadsafe, but valid
            {
                return Task.Run(() => _testsContainer.RunTestsForMutant(_choices.MutantsTestingOptions, _storedMutantInfo, mutant))
               // return RunTestsAsync()
                    .ContinueWith(task =>
                    {
                        _storedMutantInfo.Dispose();

                        return mutant.State;
                        //_sessionEventsSubject.OnNext(new MutantTestedEvent(mutant.State));

                    });
            }
            else
            {
                return Task.FromResult(MutantResultState.Untested);
            }
        }
        public Task RunTestsAsync()
        {
//            if (_cancelRequested)
//            {
//                mutant.State = MutantResultState.Killed;
//                mutant.KilledSubstate = MutantKilledSubstate.Cancelled;
//                return Task.Delay(0);
//            }
            var sw = new Stopwatch();
            sw.Start();

            mutant.State = MutantResultState.Tested;

           
            _log.Info("Loading tests for mutant " + mutant.Id);

            _timoutDisposable = Observable.Timer(TimeSpan.FromSeconds(
                _choices.MutantsTestingOptions.TestingTimeoutSeconds))
                    .Subscribe(e => CancelTestRun());

            var contexts = CreateTestContexts(_choices.AssembliesPaths,
                _choices.TestAssemblies).ToList();

            _log.Info("Running tests for mutant " + mutant.Id);
            var task = _testsContainer.RunTests(contexts);

            return task.ContinueWith(t =>
            {
                if(t.Exception != null)
                {
                    //TODO: CANCELLATION (also after timeout)
                    SetError(mutant, t.Exception.InnerException);
                    _timoutDisposable.Dispose();
                }
                else
                {
                    _log.Debug("Finished waiting for tests. ");
                    mutant.TestRunContexts = contexts;

                    _timoutDisposable.Dispose();

                    ResolveMutantState(mutant);

                    mutant.MutantTestSession.IsComplete = true;
                }
                sw.Stop();
                mutant.MutantTestSession.TestingTimeMiliseconds = sw.ElapsedMilliseconds;
            });
        }

        private void CancelTestRun()
        {
                
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

                //                testNodeAssembly.TestsLoadContext.SelectedTests = _currentSession.Choices
                //                    .TestAssemblies.Single(n => testNodeAssembly.Name == n.Name)
                //                    .TestsLoadContext.SelectedTests;
                yield return context;
            }
        }

        private void SetError(Mutant mutant, Exception e)
        {
            mutant.MutantTestSession.ErrorDescription = "Error ocurred";
            mutant.MutantTestSession.ErrorMessage = e.Message;
            mutant.MutantTestSession.Exception = e;
            mutant.State = MutantResultState.Error;
            _log.Error("Set mutant " + mutant.Id + " error: " + mutant.State + " message: " + e.Message);
        }


        private void ResolveMutantState(Mutant mutant)
        {
            List<TmpTestNodeMethod> nodeMethods = mutant.TestRunContexts
                .SelectMany(c => c.TestResults.ResultMethods).ToList();
            //.TestsByAssembly.Values.SelectMany(c => c.ClassNodes).ToList();

            mutant.NumberOfFailedTests = nodeMethods
                          .Count(t => t.State.IsIn(TestNodeState.Failure, TestNodeState.Inconclusive));


            if (nodeMethods.Any(t => t.State == TestNodeState.Inconclusive))
            {

                mutant.KilledSubstate = MutantKilledSubstate.Inconclusive;
                mutant.State = MutantResultState.Killed;
            }

            else if (nodeMethods.Any(t => t.State == TestNodeState.Failure))
            {

                mutant.KilledSubstate = MutantKilledSubstate.Normal;
                mutant.State = MutantResultState.Killed;
            }
            else if (nodeMethods.All(t => t.State == TestNodeState.Success))
            {
                mutant.State = MutantResultState.Live;
            }
            else
            {
                throw new InvalidOperationException("Unknown state");
            }
            _log.Info("Resolved mutant" + mutant.Id + " state: " + mutant.State + " sub: " + mutant.KilledSubstate);
        }

    }
}