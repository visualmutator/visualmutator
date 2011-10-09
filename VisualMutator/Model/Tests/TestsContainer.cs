namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;

    using VisualMutator.Controllers;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Tests.TestsTree;
    public class TestSession
    {
        private IDictionary<string, TestNodeMethod> _testMap;

        private List<TestNodeClass> _testClassses;


        private TestsRootNode _testsRootNode;

        public TestSession()
        {
            _testMap = new Dictionary<string, TestNodeMethod>();
            _testClassses = new List<TestNodeClass>();
        
            _testsRootNode = new TestsRootNode();
        }

        public IDictionary<string, TestNodeMethod> TestMap
        {
            get
            {
                return _testMap;
            }
        }

        public List<TestNodeClass> TestClassses
        {
            get
            {
                return _testClassses;
            }
        }

      
        public TestsRootNode TestsRootNode
        {
            get
            {
                return _testsRootNode;
            }
        }

        public IEnumerable<TestNodeNamespace> TestNamespaces
        { 
            get
            {
                return TestsRootNode.Children.Cast<TestNodeNamespace>();
            } 
        }
    }
    public interface ITestsContainer
    {
        TestSession LoadTests(MutationSession mutant);

        void RunTests(TestSession testSession);

 
        void UnloadTests();

        MutationSession CurrentMutant { get; }

        object RunTestsForMutant(Mutant mut);
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IEnumerable<ITestService> _testServices;

      

        private MutationSession _currentMutant;

        public MutationSession CurrentMutant
        {
            get
            {
                return _currentMutant;
            }
        }

        public object RunTestsForMutant(Mutant mut)
        {


            throw new NotImplementedException();
        }

        public TestsContainer(NUnitTestService nunit, MsTestService ms)
        {
            _testServices = new List<ITestService>
            {
                nunit,ms
            };

           
        }
        

        public TestSession LoadTests(MutationSession mutant)
        {
            if (mutant == null)
            {
                throw new ArgumentNullException("mutant");
            }
            _currentMutant = mutant;
      

            var testSession = new TestSession();

            var testClassses = _testServices
                .SelectMany(s => s.LoadTests(mutant.Assemblies, testSession));

            testSession.TestClassses.AddRange(testClassses);

            var testNamespaces = testSession.TestClassses
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