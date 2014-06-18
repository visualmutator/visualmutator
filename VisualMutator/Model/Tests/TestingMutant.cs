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
    using Decompilation;
    using Exceptions;
    using Infrastructure;
    using log4net;
    using Mutations.MutantsTree;
    using StoringMutants;
    using Tests;
    using Tests.Services;
    using Tests.TestsTree;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using VisualMutator.Tests.Mutations;

    public class TestingMutant
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly TestsContainer _testsContainer;
        private readonly ICodeVisualizer _codeVisualizer;
        private readonly MutantMaterializer _mutantMaterializer;
        private readonly MutationSessionChoices _choices;
        private readonly NUnitXmlTestService _nunitService;
        private readonly ISubject<SessionEventArgs> _sessionEventsSubject;
        private readonly IFactory<TestsRunContext> _testsRunContextFactory;

        private readonly Mutant _mutant;
        private StoredMutantInfo _storedMutantInfo;
        private readonly DateTime _sessionStartTime;
        private readonly OptionsModel _options;
        private List<TestsRunContext> _contexts;


        public TestingMutant(
            SessionController sessionController,
            TestsContainer testsContainer,
            ICodeVisualizer codeVisualizer,
            MutantMaterializer mutantMaterializer,
            OptionsModel options,
            MutationSessionChoices choices,
            NUnitXmlTestService nunitService,
            IFactory<TestsRunContext> testsRunContextFactory,
            //--------
            ISubject<SessionEventArgs> sessionEventsSubject,
            Mutant mutant)
        {
            _testsContainer = testsContainer;
            _codeVisualizer = codeVisualizer;
            _mutantMaterializer = mutantMaterializer;
            _options = options;
            _choices = choices;
            _nunitService = nunitService;
            _sessionEventsSubject = sessionEventsSubject;
            _testsRunContextFactory = testsRunContextFactory;
            _mutant = mutant;
            _sessionStartTime = sessionController.SessionStartTime;
        }
        public void Cancel()
        {
        }


        public async Task<MutantResultState> RunAsync()
        {
            
            var sw = new Stopwatch();
            sw.Start();
            _storedMutantInfo = await _mutantMaterializer.StoreMutant(_mutant);
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
                if (!_options.ParsedParams.DebugFiles)
                {
                    _storedMutantInfo.Dispose();
                }

                return _mutant.State;
            }
                //_sessionEventsSubject.OnNext(new MutantTestedEvent(mutant.State));
            else
            {
                return (MutantResultState.Untested);
            }
        }

        public async Task RunTestsForMutant(MutantsTestingOptions options, StoredMutantInfo storedMutantInfo)
        {
            _mutant.State = MutantResultState.Tested;

            var sw = new Stopwatch();
            sw.Start();

            _log.Info("Loading tests for mutant " + _mutant.Id);

            _contexts = CreateTestContexts(storedMutantInfo.AssembliesPaths,
                _choices.TestAssemblies).ToList();

            _log.Info("Running tests for mutant " + _mutant.Id);

           
       //     _nUnitTesters = contexts.Select(_nunitService.SpawnTester).ToList();
         //   _nUnitTesterFactory.CreateWithParams(_nunitConsolePath, arg);

            IDisposable timoutDisposable =
              Observable.Timer(TimeSpan.FromSeconds(options.TestingTimeoutSeconds))
              .Subscribe(e => CancelTestRun());

            try
            {
                await Task.WhenAll(_contexts.Select(t => t.RunTests()));
                
                _log.Debug("Finished waiting for tests. ");
                _mutant.TestRunContexts = _contexts;

                ResolveMutantState(_contexts.Select(c => c.TestResults));

                _mutant.MutantTestSession.IsComplete = true;
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                SetError(e);
            }
            finally
            {
                timoutDisposable.Dispose();
                sw.Stop();
                _mutant.MutantTestSession.TestingTimeMiliseconds = sw.ElapsedMilliseconds;
                _mutant.MutantTestSession.TestingEndRelative = DateTime.Now - _sessionStartTime;
            }
        }


        private void CancelTestRun()
        {
            foreach (var nUnitTester in _contexts)
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
                var context = _testsRunContextFactory.CreateWithParams(originalContext.SelectedTests, mutatedPath);

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