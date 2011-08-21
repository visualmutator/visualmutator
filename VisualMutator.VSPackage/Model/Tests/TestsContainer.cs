namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reactive;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;

    public interface ITestsContainer
    {
        void LoadTests(MutationSession mutant);
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IEnumerable<ITestService> _testServices;

        private IDictionary<string, TestTreeNode> _testMap;

        public BetterObservableCollection<TestNodeNamespace> TestNamespaces
        {
            set; get; 
        }

        public TestsContainer(IEnumerable<ITestService> testServices)
        {
            _testServices = testServices;

            _testMap = new Dictionary<string, TestTreeNode>();
             TestNamespaces =new BetterObservableCollection<TestNodeNamespace>();
        }

        public void Initialize()
        {
           

        }

        

        public void LoadTests(MutationSession mutant)
        {
            var assemblies = mutant.Assemblies;
            _testServices.ToObservable().e


        }

        public void RunTests()
        {
            _execute.OnUIThread(
                () =>
                {
                    _unitTestsVm.AreTestsRunning = true;
                    _commandRunTests.RaiseCanExecuteChanged();

                    foreach (TestTreeNode testTreeNode in _testMap.Values)
                    {
                        testTreeNode.Status = TestStatus.Running;
                    }
                });
        }
    }
}