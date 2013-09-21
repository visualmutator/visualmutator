namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using CommonUtilityInfrastructure.CheckboxedTree;
    using CommonUtilityInfrastructure.FunctionalUtils;
    using CommonUtilityInfrastructure.Paths;
    using Mono.Cecil;
    using Mutations.MutantsTree;
    using Mutations.Types;
    using NUnit.Core;
    using StoringMutants;
    using Verification;
    using VisualMutator.Controllers;
    using VisualMutator.Model.Exceptions;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Tests.TestsTree;

    using Switch = CommonUtilityInfrastructure.FunctionalUtils.Switch;

    public interface ITestsContainer
    {
       // TestSession LoadTests(StoredMutantInfo mutant);

        void RunTests(MutantTestSession mutantTestSession);

 
        void UnloadTests();



        void RunTestsForMutant(MutationTestingSession session, StoredMutantInfo storedMutantInfo, Mutant mutant);

        TestEnvironmentInfo InitTestEnvironment(MutationTestingSession currentSession);

        void CleanupTestEnvironment(TestEnvironmentInfo testEnvironmentInfo);

        void CancelAllTesting();

        bool VerifyMutant( StoredMutantInfo storedMutantInfo, Mutant mutant);

        StoredMutantInfo StoreMutant(TestEnvironmentInfo testEnvironment, Mutant changelessMutant);
        IEnumerable<TestNodeNamespace> LoadTests(IEnumerable<string> paths);

        ICollection<TestId> GetIncludedTests(IEnumerable<TestNodeNamespace> testNodeNamespaces);
        void CreateTestFilter(ICollection<TestId> selectedTests);
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IMutantsFileManager _mutantsFileManager;

        private readonly IAssemblyVerifier _assemblyVerifier;

        private readonly IEnumerable<ITestService> _testServices;

      

        private bool _allTestingCancelled;
        private bool _testsLoaded;

        private MutationTestingSession _currentSession;

        public TestsContainer(
            NUnitTestService nunit, 
            IMutantsFileManager mutantsFileManager,
            IAssemblyVerifier assemblyVerifier)
        {
            _mutantsFileManager = mutantsFileManager;
            _assemblyVerifier = assemblyVerifier;
            _testServices = new List<ITestService>
            {
                nunit//,ms
            };
        }
        public ICollection<TestId> GetIncludedTests(IEnumerable<TestNodeNamespace> testNodeNamespaces)
        {
            return testNodeNamespaces
                .SelectManyRecursive<NormalNode>(node => node.Children, node => node.IsIncluded ?? true, leafsOnly: true)
                .Cast<TestNodeMethod>().Select(m => m.TestId).ToList();
        
        }

        public void CreateTestFilter(ICollection<TestId> selectedTests)
        {
            foreach (var testService in _testServices)
            {
                testService.CreateTestFilter(selectedTests);
            }
        }

        public void VerifyAssemblies(List<string> assembliesPaths)
        {
            foreach (var assemblyPath in assembliesPaths)
            {
                _assemblyVerifier.Verify(assemblyPath);
            }
  
        }
        public TestEnvironmentInfo InitTestEnvironment(MutationTestingSession currentSession)
        {
            _currentSession = currentSession;
            return _mutantsFileManager.InitTestEnvironment(currentSession);
        }


        public void CleanupTestEnvironment(TestEnvironmentInfo testEnvironmentInfo)
        {
            _mutantsFileManager.CleanupTestEnvironment(testEnvironmentInfo);
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

        public StoredMutantInfo StoreMutant(TestEnvironmentInfo testEnvironment, Mutant mutant)
        {
            return _mutantsFileManager.StoreMutant(testEnvironment.DirectoryPath, mutant);
        }
        public IEnumerable<TestNodeNamespace> LoadTests(IEnumerable<string> paths)
        {
            var session = new MutantTestSession();
            LoadTests(paths, session);
            UnloadTests();

            var root = new FakeNode();
            root.Children.AddRange(session.TestNamespaces);
            root.IsIncluded = true;

            return session.TestNamespaces;
        }

        public void RunTestsForMutant(MutationTestingSession session, StoredMutantInfo storedMutantInfo, Mutant mutant)
        {
            if (_allTestingCancelled)
            {
                return;
            }
            bool testsLoaded = false;
            var sw = new Stopwatch();
            sw.Start();

            mutant.State = MutantResultState.Tested;

            IDisposable timoutDisposable = null;
            try
            {
               
                LoadTests(storedMutantInfo.AssembliesPaths, mutant.MutantTestSession);

                testsLoaded = true;

                timoutDisposable = Observable.Timer(TimeSpan.FromSeconds(session.Choices
                    .MutantsTestingOptions.TestingTimeoutSeconds)).Subscribe(e => CancelCurrentTestRun());

                RunTests(mutant.MutantTestSession);

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
        }
        private void ResolveMutantState(Mutant mutant)
        {
            mutant.NumberOfFailedTests = mutant.MutantTestSession.TestMap.Values
                          .Count(t => t.State.IsIn(TestNodeState.Failure, TestNodeState.Inconclusive));



            if (mutant.MutantTestSession.TestMap.Values.Any(t => t.State == TestNodeState.Inconclusive))
            {
                
                mutant.KilledSubstate = MutantKilledSubstate.Inconclusive;
                mutant.State = MutantResultState.Killed;
            }

            else if (mutant.MutantTestSession.TestMap.Values.Any(t => t.State == TestNodeState.Failure))
            {
              
                mutant.KilledSubstate = MutantKilledSubstate.Normal;
                mutant.State = MutantResultState.Killed;
            }
            else if (mutant.MutantTestSession.TestMap.Values.All(t => t.State == TestNodeState.Success))
            {
                mutant.State = MutantResultState.Live;
            }
            else
            {
                throw new InvalidOperationException("Unknown state");
            }

        }

        public void CancelAllTesting()
        {
            _allTestingCancelled = true;
            CancelCurrentTestRun();
        }

        private void CancelCurrentTestRun()
        {
            foreach (var service in _testServices)
            {
                service.Cancel();

                _currentSession.Choices.MutantsTestingOptions
                    .TestingProcessExtensionOptions.TestingProcessExtension.OnTestingCancelled();
               // _mutantsFileManager.OnTestingCancelled();
            }
        }

        public void LoadTests(IEnumerable<string> assembliesPaths, MutantTestSession mutantTestSession)
        {
            Throw.IfNull(assembliesPaths, "assembliesPaths");
           

            IEnumerable<TestNodeClass> testClassses = _testServices
                .SelectMany(s => s.LoadTests(assembliesPaths, mutantTestSession));

            var r = testClassses.Where(t => t.Namespace == null);
            List<TestNodeNamespace> testNamespaces = testClassses
                .GroupBy(classNode => classNode.Namespace)
                .Select(group =>
                {
                    var ns = new TestNodeNamespace(mutantTestSession.TestsRootNode, group.Key);
                    foreach (TestNodeClass nodeClass in group)
                    {
                        nodeClass.Parent = ns;
                    }
       
                    ns.Children.AddRange(group);
                    return ns;

                }).ToList();


            mutantTestSession.TestsRootNode.Children.AddRange(testNamespaces);
            mutantTestSession.TestsRootNode.State = TestNodeState.Inactive;

            _testsLoaded = true;
        }



        public void RunTests(MutantTestSession mutantTestSession)
        {
            mutantTestSession.TestsRootNode.State = TestNodeState.Running;

            foreach (var service in _testServices)
            {
                service.RunTests(mutantTestSession);
            }
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