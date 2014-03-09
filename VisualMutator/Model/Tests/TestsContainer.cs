namespace VisualMutator.Model.Tests
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Exceptions;
    using Infrastructure;
    using LinqLib.Operators;
    using log4net;
    using Mutations;
    using Mutations.MutantsTree;
    using Services;
    using StoringMutants;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.ExtensionMethods;
    using Verification;

    #endregion

    public interface ITestsContainer
    {
       // TestSession LoadTests(StoredMutantInfo mutant);

        Task RunTests(List<TestsRunContext> testContexts);

 


        void RunTestsForMutant(MutantsTestingOptions session, StoredMutantInfo storedMutantInfo, 
            Mutant mutant);

        ProjectFilesClone InitTestEnvironment();


        void CancelAllTesting();

        bool VerifyMutant( StoredMutantInfo storedMutantInfo, Mutant mutant);

        StoredMutantInfo StoreMutant(ProjectFilesClone testEnvironment, Mutant changelessMutant);
     //   TestsRootNode LoadTestsPublic(IEnumerable<string> paths);

   
        void CreateTestSelections(IList<TestNodeAssembly> testAssemblies);
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IMutantsFileManager _mutantsFileManager;
        private readonly IFileSystemManager _fileManager;

        private readonly IAssemblyVerifier _assemblyVerifier;
        private readonly MutationSessionChoices _choices;

        private readonly IEnumerable<ITestService> _testServices;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool _allTestingCancelled;
        private bool _testsLoaded;

        private readonly TestResultTreeCreator _testResultTreeCreator;

        public TestsContainer(
            NUnitXmlTestService nunit, 
            IMutantsFileManager mutantsFileManager,
            IFileSystemManager fileManager,
            IAssemblyVerifier assemblyVerifier,
            MutationSessionChoices choices)
        {
            _mutantsFileManager = mutantsFileManager;
            _fileManager = fileManager;
            _assemblyVerifier = assemblyVerifier;
            _choices = choices;
            _testServices = new List<ITestService>
            {
                nunit//,ms
            };
            _testResultTreeCreator = new TestResultTreeCreator();
        }
       

        public void CreateTestSelections(IList<TestNodeAssembly> testAssemblies)
        {
            var testsSelector = new TestsSelector();
            foreach (var testNodeAssembly in testAssemblies)
            {
                testNodeAssembly.TestsLoadContext.SelectedTests = 
                    testsSelector.GetIncludedTests(testNodeAssembly);
            }
        }


        public void VerifyAssemblies(List<string> assembliesPaths)
        {
            foreach (var assemblyPath in assembliesPaths)
            {
                _assemblyVerifier.Verify(assemblyPath);
            }
  
        }
        public ProjectFilesClone InitTestEnvironment()
        {
            return _fileManager.CreateClone("InitTestEnvironment");
        }



        public bool VerifyMutant( StoredMutantInfo storedMutantInfo, Mutant mutant)
        {

            try
            {
                VerifyAssemblies(storedMutantInfo.AssembliesPaths);
            }
            catch (AssemblyVerificationException e)
            {

                mutant.MutantTestSession.ErrorDescription = "Mutant assembly failed verification";
                mutant.MutantTestSession.ErrorMessage = e.Message;
                mutant.MutantTestSession.Exception = e;
                mutant.State = MutantResultState.Error;
                return false;
            }
            return true;
                

        }

        public StoredMutantInfo StoreMutant(ProjectFilesClone testEnvironment, Mutant mutant)
        {
            return _mutantsFileManager.StoreMutant(testEnvironment.ParentPath.Path,  mutant);
        }
//        public TestsRootNode LoadTestsPublic(IEnumerable<string> paths)
//        {
//          //  var mutantTestSession = new MutantTestSession();
//            var root = LoadTests(paths.ToList());
//            
//            
//           // session.TestsByAssembly
//           // var root = new RootNode();
//            //root.Children.AddRange(session.TestNamespaces);
//            //root.IsIncluded = true;
//
//            return root;
//        }

        public void RunTestsForMutant(MutantsTestingOptions options,
            StoredMutantInfo storedMutantInfo, Mutant mutant)
        {
            if (_allTestingCancelled)
            {
                mutant.State = MutantResultState.Killed;
                mutant.KilledSubstate = MutantKilledSubstate.Cancelled;
                return;
            }
            bool testsLoaded = false;
            var sw = new Stopwatch();
            sw.Start();

            mutant.State = MutantResultState.Tested;

            IDisposable timoutDisposable = null;
            try
            {
                ITestService testService = _testServices.Single();

                _log.Info("Loading tests for mutant " + mutant.Id);
               // LoadTests(storedMutantInfo.AssembliesPaths, mutant.MutantTestSession);

                testsLoaded = true;

                timoutDisposable = Observable.Timer(TimeSpan.FromSeconds(options.TestingTimeoutSeconds))
                    .Subscribe(e => CancelCurrentTestRun());

                var contexts = CreateTestContexts(storedMutantInfo.AssembliesPaths,
                    _choices.TestAssemblies).ToList();

                _log.Info("Running tests for mutant " + mutant.Id);
                var task = RunTests(contexts);
                task.Wait();
                _log.Debug("Finished waiting for tests. ");
                mutant.TestRunContexts = contexts;

                timoutDisposable.Dispose();

                ResolveMutantState(mutant);

                mutant.MutantTestSession.IsComplete = true;
            }
            catch (TestingCancelledException)
            {
                mutant.KilledSubstate = MutantKilledSubstate.Cancelled;
                mutant.State = MutantResultState.Killed;
                
            }
            catch (Exception e)
            {

                SetError(mutant, e);
            }
            finally
            {
            
                
                if (timoutDisposable != null)
                {
                    timoutDisposable.Dispose();
                }
                sw.Stop();
                mutant.MutantTestSession.TestingTimeMiliseconds = sw.ElapsedMilliseconds; 
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
            _log.Info("Resolved mutant"+mutant.Id+" state: " + mutant.State + " sub: " + mutant.KilledSubstate);
        }

        public void CancelAllTesting()
        {
            _log.Info("Request to cancel all testing.");
            _allTestingCancelled = true;
            CancelCurrentTestRun();
        }

        private void CancelCurrentTestRun()
        {
            foreach (var service in _testServices)
            {
                service.Cancel();
                if (_choices.MutantsTestingOptions
                    .TestingProcessExtensionOptions.TestingProcessExtension != null)
                {
                    _choices.MutantsTestingOptions
                        .TestingProcessExtensionOptions.TestingProcessExtension.OnTestingCancelled();
                }
            }
        }

        public Task RunTests(List<TestsRunContext> testContexts)
        {
            var service = _testServices.Single();
            List<Task> tasks = testContexts.Select(service.RunTests).ToList();
            _log.Debug("Waiting for " + tasks.Count + " test task. ");
            return Task.WhenAll(tasks);
        }

        public IEnumerable<TestNodeNamespace> CreateMutantTestTree(Mutant mutant)
        {
            List<TmpTestNodeMethod> nodeMethods = mutant.TestRunContexts
                .SelectMany(c => c.TestResults.ResultMethods).ToList();

            return _testResultTreeCreator.CreateMutantTestTree(nodeMethods);
        }
    }
}