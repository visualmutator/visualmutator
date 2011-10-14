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
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Tests.TestsTree;

    public interface ITestsContainer
    {
       // TestSession LoadTests(StoredMutantInfo mutant);

        void RunTests(TestSession testSession);

 
        void UnloadTests();

        StoredMutantInfo CurrentMutant { get; }

        Mutant RunTestsForMutant(Mutant mutant);
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IMutantsFileManager _mutantsFileManager;

        private readonly CommonUtilityInfrastructure.Services _services;

        private readonly IEnumerable<ITestService> _testServices;

      

        private StoredMutantInfo _currentMutant;

        public StoredMutantInfo CurrentMutant
        {
            get
            {
                return _currentMutant;
            }
        }

      

        public TestsContainer(NUnitTestService nunit, MsTestService ms,
            IMutantsFileManager mutantsFileManager, CommonUtilityInfrastructure.Services services)
        {
            _mutantsFileManager = mutantsFileManager;
            _services = services;
            _testServices = new List<ITestService>
            {
                nunit,ms
            };
        }

        public Mutant RunTestsForMutant(Mutant mutant)
        {
            mutant.State = MutantResultState.Tested;
            StoredMutantInfo storedMutantInfo = _mutantsFileManager.StoreMutant(mutant);

            TestSession testSession = LoadTests(storedMutantInfo);



            _services.Threading.InvokeOnGui(() => mutant.TestSession = testSession);

            RunTests(testSession);


            UnloadTests();
            _mutantsFileManager.DeleteMutantFiles(storedMutantInfo);


            mutant.State = testSession.TestsRootNode.State == TestNodeState.Failure
                               ? MutantResultState.Killed : MutantResultState.Live;
            return mutant;
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
                    group.Each(c => c.Parent = ns);
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