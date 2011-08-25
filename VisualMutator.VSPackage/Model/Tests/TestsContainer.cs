namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reactive;
    using System.Threading;
    using System.Threading.Tasks;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;

    public interface ITestsContainer
    {
        IEnumerable<TestNodeNamespace> LoadTests(MutationSession mutant);

        void RunTests();
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IEnumerable<ITestService> _testServices;

   

        public BetterObservableCollection<TestNodeNamespace> TestNamespaces
        {
            set; get; 
        }

        public TestsContainer(NUnitTestService nunit, MsTestService ms)
        {
            _testServices = new List<ITestService>
            {
                nunit,ms
            };



            /*
            if (_testServices == null || _testServices.Count() == 0)
            {
                throw new ArgumentNullException("testServices");
            }
          */
             TestNamespaces =new BetterObservableCollection<TestNodeNamespace>();
        }

        public void Initialize()
        {
           

        }

        

        public IEnumerable<TestNodeNamespace> LoadTests(MutationSession mutant)
        {
        
            return _testServices.AsParallel()
                .SelectMany(s => s.LoadTests(mutant.Assemblies))
                .GroupBy(classNode => classNode.Namespace)
                .Select(group => new TestNodeNamespace
                {
                    Name = group.Key,
                    TestClasses = group.ToObsCollection()
                }).ToList();


        }

        public void RunTests()
        {
     
            Parallel.ForEach(_testServices, service =>
            {
                service.RunTests();
            });


        }
    }
}