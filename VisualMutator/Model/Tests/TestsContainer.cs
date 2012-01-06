namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;

    using Mono.Cecil;

    using VisualMutator.Controllers;
    using VisualMutator.Model.Exceptions;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Tests.TestsTree;

    using Switch = CommonUtilityInfrastructure.Switch;

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
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IMutantsFileManager _mutantsFileManager;

        private readonly CommonServices _commonServices;

        private readonly IAssemblyVerifier _assemblyVerifier;

        private readonly IEnumerable<ITestService> _testServices;

      

        private StoredMutantInfo _currentMutant;

        private bool _allTestingCancelled;

        public TestsContainer(
            NUnitTestService nunit, 
            MsTestService ms,
            IMutantsFileManager mutantsFileManager, 
            CommonServices commonServices,
            IAssemblyVerifier assemblyVerifier)
        {
            _mutantsFileManager = mutantsFileManager;
            _commonServices = commonServices;
            _assemblyVerifier = assemblyVerifier;
            _testServices = new List<ITestService>
            {
                nunit,ms
            };
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
           
            return _mutantsFileManager.InitTestEnvironment(currentSession);
        }


        public void CleanupTestEnvironment(TestEnvironmentInfo testEnvironmentInfo)
        {
            _mutantsFileManager.CleanupTestEnvironment(testEnvironmentInfo);
        }


        public bool VerifyMutant( StoredMutantInfo storedMutantInfo, Mutant mutant)
        {

          //  StoredMutantInfo storedMutantInfo = _mutantsFileManager.StoreMutant(testEnvironmentInfo, mutant);

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

        public StoredMutantInfo StoreMutant(TestEnvironmentInfo testEnvironment, Mutant changelessMutant)
        {
            return _mutantsFileManager.StoreMutant(testEnvironment.DirectoryPath, changelessMutant);
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
               
                LoadTests(storedMutantInfo, mutant.MutantTestSession);

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
                _mutantsFileManager.OnTestingCancelled();
            }
        }

        public void LoadTests(StoredMutantInfo mutant, MutantTestSession mutantTestSession)
        {
            Throw.IfArgumentNull(mutant, "mutant");
           
            _currentMutant = mutant;
      

            IEnumerable<TestNodeClass> testClassses = _testServices
                .SelectMany(s => s.LoadTests(mutant.AssembliesPaths, mutantTestSession));

            mutantTestSession.TestClassses.AddRange(testClassses);

            List<TestNodeNamespace> testNamespaces = mutantTestSession.TestClassses
                .GroupBy(classNode => classNode.Namespace)
                .Select(group =>
                {
                    var ns = new TestNodeNamespace(mutantTestSession.TestsRootNode, group.Key);
                    foreach (var nodeClass in group)
                    {
                        nodeClass.Parent = ns;
                    }
       
                    ns.Children.AddRange(group);
                    return ns;

                }).ToList();


            mutantTestSession.TestsRootNode.Children.AddRange(testNamespaces);
            mutantTestSession.TestsRootNode.State = TestNodeState.Inactive;

          
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
            if (_currentMutant != null)
            {
                foreach (ITestService testService in _testServices)
                {
                    testService.UnloadTests();
                }
                _currentMutant = null;
            }
            
        }

    }
}