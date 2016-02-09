namespace VisualMutator.Model.Tests
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
    using log4net;
    using Mutations;
    using Mutations.MutantsTree;
    using StoringMutants;
    using Tests.Services;
    using Tests.TestsTree;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;

    public class TestingMutant
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MutantMaterializer _mutantMaterializer;
        private readonly MutationSessionChoices _choices;
        private readonly IObserver<SessionEventArgs> _sessionEventsSubject;
        private readonly TestServiceManager _testServiceManager;

        private readonly Mutant _mutant;
        private StoredMutantInfo _storedMutantInfo;
        private readonly OptionsModel _options;
        private List<ITestsRunContext> _contexts;


        public TestingMutant(
            MutantMaterializer mutantMaterializer,
            OptionsModel options,
            MutationSessionChoices choices,
            NUnitXmlTestService testService,
            TestServiceManager testServiceManager,
            //--------
            IObserver<SessionEventArgs> sessionEventsSubject,
            Mutant mutant)
        {
            _mutantMaterializer = mutantMaterializer;
            _options = options;
            _choices = choices;
            _sessionEventsSubject = sessionEventsSubject;
            _testServiceManager = testServiceManager;
            _mutant = mutant;
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

            if (_choices.TestAssemblies.Count == 0)
            {
                throw new InvalidOperationException("_choices.TestAssemblies.Count == 0");
            }

            _contexts = CreateTestContexts(storedMutantInfo.AssembliesPaths,
                _choices.TestAssemblies).ToList();
           

            _log.Info("Running tests for mutant " + _mutant.Id);

           
       //     _nUnitTesters = contexts.Select(_nunitService.SpawnTester).ToList();
         //   _nUnitTesterFactory.CreateWithParams(_nunitConsolePath, arg);

            IDisposable timoutDisposable =
              Observable.Timer(TimeSpan.FromMilliseconds(options.TestingTimeoutSeconds))
              .Subscribe(e => CancelTestRun());

            try
            {
                var results = await Task.WhenAll(_contexts.Select(t => t.RunTests()));
                
                _log.Debug("Finished waiting for tests. ");
                _mutant.TestRunContexts = _contexts;

                ResolveMutantState(results);

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
                _mutant.MutantTestSession.TestingEnd = DateTime.Now;
            }
        }


        private void CancelTestRun()
        {
            foreach (var nUnitTester in _contexts)
            {
                nUnitTester.CancelRun();
            }
        }


        private IEnumerable<ITestsRunContext> CreateTestContexts(
            List<string> mutatedPaths,
            IList<TestNodeAssembly> testAssemblies)
        {
            foreach (var testNodeAssembly in testAssemblies)
            {
                //todo: get rid of this ungly thing

                var mutatedPath = mutatedPaths.Single(p => Path.GetFileName(p) ==
                    Path.GetFileName(testNodeAssembly.AssemblyPath));

                foreach (TestsLoadContext loadContext in testNodeAssembly.TestsLoadContexts)
                {
                   
                    yield return _testServiceManager.CreateRunContext(loadContext, mutatedPath);
                  //  TestsRunContext context = _testsRunContextFactory.CreateWithParams(loadContext, mutatedPath);
                 //   yield return context;
                }
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

            var count = nodeMethods
                  .Select(t => t.State).GroupBy(t => t)
                  .ToDictionary(t => t.Key, t => t.Count());
            var countStrings = count.Select(pair => pair.Key.ToString() + ": " + pair.Value);
            _log.Info(string.Format("All test results: "+ string.Join(" ",countStrings)));

            _mutant.NumberOfFailedTests = 
                count.GetOrDefault(TestNodeState.Failure) 
                + count.GetOrDefault(TestNodeState.Inconclusive);
                       

            if (count.GetOrDefault(TestNodeState.Inconclusive) > 0)
            {
                _mutant.KilledSubstate = MutantKilledSubstate.Inconclusive;
                _mutant.State = MutantResultState.Killed;
            }

            else if (count.GetOrDefault(TestNodeState.Failure) > 0)
            {
                _mutant.KilledSubstate = MutantKilledSubstate.Normal;
                _mutant.State = MutantResultState.Killed;
            }
            else if (count.GetOrDefault(TestNodeState.Success) 
                + count.GetOrDefault(TestNodeState.Inactive) == nodeMethods.Count)
            {
                _mutant.State = MutantResultState.Live;
            }
            else
            {
                throw new InvalidOperationException("Dont know how to resolve mutant state based on tests.");
            }
            _log.Info("Resolved mutant" + _mutant.Id + " state: " + _mutant.State + " sub: " + _mutant.KilledSubstate);
        }

    }
}