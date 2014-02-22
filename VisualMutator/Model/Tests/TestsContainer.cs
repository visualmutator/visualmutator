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

        Task RunTests(MutantTestSession mutantTestSession);

 
        void UnloadTests();



        void RunTestsForMutant(MutantsTestingOptions session, StoredMutantInfo storedMutantInfo, 
            Mutant mutant);

        ProjectFilesClone InitTestEnvironment(MutationTestingSession currentSession);


        void CancelAllTesting();

        bool VerifyMutant( StoredMutantInfo storedMutantInfo, Mutant mutant);

        StoredMutantInfo StoreMutant(ProjectFilesClone testEnvironment, Mutant changelessMutant);
        IEnumerable<TestNodeAssembly> LoadTests(IEnumerable<string> paths);

   
        void CreateTestSelections(IList<TestNodeAssembly> testAssemblies);
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IMutantsFileManager _mutantsFileManager;
        private readonly IFileSystemManager _fileManager;

        private readonly IAssemblyVerifier _assemblyVerifier;

        private readonly IEnumerable<ITestService> _testServices;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool _allTestingCancelled;
        private bool _testsLoaded;

        private MutationTestingSession _currentSession;

        public TestsContainer(
            NUnitXmlTestService nunit, 
            IMutantsFileManager mutantsFileManager,
            IFileSystemManager fileManager,
            IAssemblyVerifier assemblyVerifier)
        {
            _mutantsFileManager = mutantsFileManager;
            _fileManager = fileManager;
            _assemblyVerifier = assemblyVerifier;
            _testServices = new List<ITestService>
            {
                nunit//,ms
            };
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
        public ProjectFilesClone InitTestEnvironment(MutationTestingSession currentSession)
        {
            _currentSession = currentSession;
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
        public IEnumerable<TestNodeAssembly> LoadTests(IEnumerable<string> paths)
        {
            var mutantTestSession = new MutantTestSession();
            LoadTests(paths.ToList(), mutantTestSession);
            UnloadTests();
            
            
           // session.TestsByAssembly
           // var root = new RootNode();
            //root.Children.AddRange(session.TestNamespaces);
            //root.IsIncluded = true;

            return mutantTestSession.TestsRootNode.TestNodeAssemblies;
        }

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
                LoadTests(storedMutantInfo.AssembliesPaths, mutant.MutantTestSession);

                testsLoaded = true;

                timoutDisposable = Observable.Timer(TimeSpan.FromSeconds(options.TestingTimeoutSeconds))
                    .Subscribe(e => CancelCurrentTestRun());

                //todo: get rid of this ungly thing
                foreach (var testNodeAssembly in mutant.MutantTestSession.TestsRootNode.TestNodeAssemblies)
                {
                    testNodeAssembly.TestsLoadContext.SelectedTests = _currentSession.Choices
                        .TestAssemblies.Single(n => testNodeAssembly.Name == n.Name)
                        .TestsLoadContext.SelectedTests;
                }


                _log.Info("Running tests for mutant " + mutant.Id);
                Task runTests = RunTests(mutant.MutantTestSession);
                runTests.Wait();

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
                if (testsLoaded)
                {
                    UnloadTests();
                }
                
                if (timoutDisposable != null)
                {
                    timoutDisposable.Dispose();
                }
                sw.Stop();
                mutant.MutantTestSession.TestingTimeMiliseconds = sw.ElapsedMilliseconds; 
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
            List<TestNodeClass> testNodeClasses = mutant.MutantTestSession
                .TestsByAssembly.Values.SelectMany(c => c.ClassNodes).ToList();

            mutant.NumberOfFailedTests = testNodeClasses
                          .Count(t => t.State.IsIn(TestNodeState.Failure, TestNodeState.Inconclusive));


            if (testNodeClasses.Any(t => t.State == TestNodeState.Inconclusive))
            {
                
                mutant.KilledSubstate = MutantKilledSubstate.Inconclusive;
                mutant.State = MutantResultState.Killed;
            }

            else if (testNodeClasses.Any(t => t.State == TestNodeState.Failure))
            {
              
                mutant.KilledSubstate = MutantKilledSubstate.Normal;
                mutant.State = MutantResultState.Killed;
            }
            else if (testNodeClasses.All(t => t.State == TestNodeState.Success))
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
                if (_currentSession.Choices.MutantsTestingOptions
                    .TestingProcessExtensionOptions.TestingProcessExtension != null)
                {
                    _currentSession.Choices.MutantsTestingOptions
                        .TestingProcessExtensionOptions.TestingProcessExtension.OnTestingCancelled();
                }
            }
        }

        public void LoadTests(IList<string> assembliesPaths, MutantTestSession mutantTestSession)
        {
            Throw.IfNull(assembliesPaths, "assembliesPaths");

            var sw = new Stopwatch();
            sw.Start();
            var tasks = new Dictionary<string, Task<May<TestNodeAssembly>>>();
            

            ITestService service1 = _testServices.Single();
            foreach (var path in assembliesPaths)
            {
                string path1 = path;
                Task<May<TestNodeAssembly>> task = Task.Run(() => service1.LoadTests(path1))
                    .ContinueWith(result =>
                    {
                        if(!result.Result.HasValue)
                        {
                            return May.NoValue;
                        }
                        string assemblyName = Path.GetFileNameWithoutExtension(path1);
                        string assemblyPath = path1;
                        TestsLoadContext context = result.Result.ForceGetValue();
                        context.TestNodeAssembly = new TestNodeAssembly(mutantTestSession.TestsRootNode, assemblyName);
                        context.TestNodeAssembly.AssemblyPath = assemblyPath;
                        context.TestNodeAssembly.TestsLoadContext = context;

                        List<TestNodeNamespace> testNamespaces = context.ClassNodes
                            .GroupBy(classNode => classNode.Namespace)
                            .Select(group =>
                            {
                                var ns = new TestNodeNamespace(context.TestNodeAssembly, group.Key);
                                foreach (TestNodeClass nodeClass in group)
                                {
                                    nodeClass.Parent = ns;
                                }

                                ns.Children.AddRange(group);
                                return ns;

                            }).ToList();

                        context.TestNodeAssembly.Children.AddRange(testNamespaces);
                        return new May<TestNodeAssembly>(context.TestNodeAssembly);
                    });
                tasks.Add(path, task);

            }
            List<TestNodeAssembly> testNodeAssemblies = Task.WhenAll(tasks.Values).Result
                .WhereHasValue().ToList();

            sw.Stop();
            mutantTestSession.LoadTestsTimeRawMiliseconds = sw.ElapsedMilliseconds;


            mutantTestSession.TestsRootNode.Children.AddRange(testNodeAssemblies);
            mutantTestSession.TestsRootNode.State = TestNodeState.Inactive;
            mutantTestSession.TestsRootNode.IsIncluded = true;
            _testsLoaded = true;
        }



        public Task RunTests(MutantTestSession mutantTestSession)
        {
            mutantTestSession.TestsRootNode.State = TestNodeState.Running;
            var service = _testServices.Single();

            List<Task> tasks = mutantTestSession.TestsRootNode.TestNodeAssemblies
                .Select(a => service.RunTests(a.TestsLoadContext)).ToList();

            return Task.WhenAll(tasks);
        }


        public void UnloadTests()
        {
            if (_testsLoaded)
            {
                foreach (ITestService testService in _testServices)
                {
                    testService.UnloadTests();
                }
                _testsLoaded = false;
            }
            
        }

    }
}