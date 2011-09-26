namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;

    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Tests.TestsTree;

    public interface ITestsContainer
    {
        IEnumerable<TestNodeNamespace> LoadTests(MutationSession mutant);

        void RunTests();

 
        void UnloadTests();

        MutationSession CurrentMutant { get; }
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IEnumerable<ITestService> _testServices;

        private TestsRootNode _testsRootNode;

        private MutationSession _currentMutant;

        public MutationSession CurrentMutant
        {
            get
            {
                return _currentMutant;
            }
        }

        public TestsContainer(NUnitTestService nunit, MsTestService ms)
        {
            _testServices = new List<ITestService>
            {
                nunit,ms
            };

           
        }
        public TestsRootNode TestsRootNode
        {
            get
            {
                return _testsRootNode;
            }
        }
    
       


        public IEnumerable<TestNodeNamespace> LoadTests(MutationSession mutant)
        {
            _currentMutant = mutant;
            _testsRootNode = new TestsRootNode();

            IEnumerable<TestNodeNamespace> namespaces =
                _testServices.AsParallel()
                .SelectMany(s => s.LoadTests(mutant.Assemblies))
                .GroupBy(classNode => classNode.Namespace)
                .Select(group =>
                {
                    var ns = new TestNodeNamespace(_testsRootNode, group.Key);
                    group.Each(c => c.Parent = ns);
                    ns.Children.AddRange(group);
                    return ns;

                }).ToList();
            
            _testsRootNode.Children.AddRange(namespaces);
          
            _testsRootNode.State = TestNodeState.Inactive;

            return namespaces;
        }



        public void RunTests()
        {   
            _testsRootNode.State = TestNodeState.Running;
            Parallel.ForEach(_testServices, service =>
            {
                service.RunTests();
            });
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