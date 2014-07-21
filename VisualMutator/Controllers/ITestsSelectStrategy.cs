namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Model;
    using Model.CoverageFinder;
    using Model.Tests.TestsTree;
    using UsefulTools.ExtensionMethods;

    public interface ITestsSelectStrategy
    {
        Task<List<TestNodeAssembly>> SelectTests();
    }

    public class AllTestsSelectStrategy : ITestsSelectStrategy
    {
        private readonly Task<object> _testsTask;

        public AllTestsSelectStrategy( Task<object> testsTask)
        {
            _testsTask = testsTask;
        }

        public async Task<List<TestNodeAssembly>> SelectTests()
        {
            var testsRootNode = (TestsRootNode) await _testsTask;
            testsRootNode.IsIncluded = true;
            return testsRootNode.TestNodeAssemblies.ToList();
        }
    }

    public class CoveringTestsSelectStrategy : ITestsSelectStrategy
    {
        private readonly Task<List<CciModuleSource>> _assembliesTask;
        private readonly ICodePartsMatcher _matcher;
        private readonly Task<object> _testsTask;

        public CoveringTestsSelectStrategy(Task<List<CciModuleSource>> assembliesTask,
            ICodePartsMatcher matcher, Task<object> testsTask)
        {
            _assembliesTask = assembliesTask;
            _matcher = matcher;
            _testsTask = testsTask;
        }

        public async Task<List<TestNodeAssembly>> SelectTests()
        {
            var finder = new CoveringTestsFinder();
            List<CciModuleSource> modules = await _assembliesTask;
            var coveringTask = finder.FindCoveringTests(modules, _matcher);

            var result = await TupleExtensions.WhenAll(Tuple.Create(coveringTask, _testsTask));
            var coveringTests = result.Item1;
            var testsRootNode = (TestsRootNode)result.Item2;

            SelectOnlyCoveredTests(testsRootNode, coveringTests);

            TreeUtils.ExpandLoneNodes(testsRootNode);

            return testsRootNode.TestNodeAssemblies.ToList();
        }



        private void SelectOnlyCoveredTests(TestsRootNode rootNode, List<MethodIdentifier> coveredTests)
        {
            rootNode.IsIncluded = false;
            var toSelect = rootNode.Children.SelectManyRecursive(n => n.Children, leafsOnly: true)
                .OfType<TestNodeMethod>()
                .Where(t => coveredTests.Contains(t.Identifier));
            foreach (var testNodeMethod in toSelect)
            {
                testNodeMethod.IsIncluded = true;
            }
        }
    }
}