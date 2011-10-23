namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;

    using Mono.Cecil;

    using VisualMutator.Controllers;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Tests.TestsTree;

    public interface ITestsContainer
    {
       // TestSession LoadTests(StoredMutantInfo mutant);

        void RunTests(TestSession testSession);

 
        void UnloadTests();

   

        void RunTestsForMutant(TestEnvironmentInfo testEnvironmentInfo, Mutant mutant);

        TestEnvironmentInfo InitTestEnvironment();
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IMutantsFileManager _mutantsFileManager;

        private readonly CommonUtilityInfrastructure.CommonServices _commonServices;

        private readonly IEnumerable<ITestService> _testServices;

      

        private StoredMutantInfo _currentMutant;

     

        public TestsContainer(NUnitTestService nunit, MsTestService ms,
            IMutantsFileManager mutantsFileManager, CommonUtilityInfrastructure.CommonServices commonServices)
        {
            _mutantsFileManager = mutantsFileManager;
            _commonServices = commonServices;
            _testServices = new List<ITestService>
            {
                nunit,ms
            };
        }

        public void RunTestsForMutant(TestEnvironmentInfo testEnvironmentInfo, Mutant mutant)
        {
            mutant.State = MutantResultState.Tested;
            StoredMutantInfo storedMutantInfo = _mutantsFileManager.StoreMutant(testEnvironmentInfo, mutant);

            TestSession testSession = LoadTests(storedMutantInfo);



            _commonServices.Threading.InvokeOnGui(() => mutant.TestSession = testSession);

            RunTests(testSession);


            UnloadTests();
            _mutantsFileManager.DeleteMutantFiles(storedMutantInfo);


            if (testSession.TestsRootNode.State == TestNodeState.Failure)
            {
                mutant.NumberOfTestsThatKilled = testSession.TestMap.Values
                    .Count(t => t.State == TestNodeState.Failure);

                mutant.State = MutantResultState.Killed;
    
            }
            else
            {
                mutant.State = MutantResultState.Live;
           
            }

            mutant.TestSession.IsComplete = true;
        }

        public TestEnvironmentInfo InitTestEnvironment()
        {
            return _mutantsFileManager.InitTestEnvironment();
        }

        public TestSession LoadTests(StoredMutantInfo mutant)
        {
            if (mutant == null)
            {
                throw new ArgumentNullException("mutant");
            }
            _currentMutant = mutant;
      

            var testSession = new TestSession();

            IEnumerable<TestNodeClass> testClassses = _testServices
                .SelectMany(s => s.LoadTests(mutant.AssembliesPaths, testSession));

            testSession.TestClassses.AddRange(testClassses);

            List<TestNodeNamespace> testNamespaces = testSession.TestClassses
                .GroupBy(classNode => classNode.Namespace)
                .Select(group =>
                {
                    var ns = new TestNodeNamespace(testSession.TestsRootNode, group.Key);
                    foreach (var nodeClass in group)
                    {
                        nodeClass.Parent = ns;
                    }
       
                    ns.Children.AddRange(group);
                    return ns;

                }).ToList();


            testSession.TestsRootNode.Children.AddRange(testNamespaces);
            testSession.TestsRootNode.State = TestNodeState.Inactive;

            return testSession;
        }



        public void RunTests(TestSession testSession)
        {
            testSession.TestsRootNode.State = TestNodeState.Running;

            foreach (var service in _testServices)
            {
                service.RunTests(testSession);
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