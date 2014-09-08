namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Model;
    using Model.CoverageFinder;
    using Model.Tests.TestsTree;
    using UsefulTools.ExtensionMethods;

    public interface ITestsSelectStrategy
    {
        Task<List<TestNodeAssembly>> SelectTests(List<string> testAssemblies);
    }

    public class AllTestsSelectStrategy : ITestsSelectStrategy
    {
        private readonly Task<TestsRootNode> _testsTask;

        public AllTestsSelectStrategy( Task<TestsRootNode> testsTask)
        {
            _testsTask = testsTask;
        }

        public async Task<List<TestNodeAssembly>> SelectTests(List<string> testAssemblies = null)
        {
            var testsRootNode = await _testsTask;
            testsRootNode.IsIncluded = true;
            if(testAssemblies != null)
            {
                return testsRootNode.TestNodeAssemblies.Where(a => 
                    testAssemblies.Select(Path.GetFileNameWithoutExtension).Contains(a.Name)).ToList();
            }
            else
            {
                return testsRootNode.TestNodeAssemblies.ToList();
            }
        }
    }

    public class CoveringTestsSelectStrategy : ITestsSelectStrategy
    {
        private readonly Task<List<CciModuleSource>> _assembliesTask;
        private readonly ICodePartsMatcher _matcher;
        private readonly Task<TestsRootNode> _testsTask;

        public CoveringTestsSelectStrategy(Task<List<CciModuleSource>> assembliesTask,
            ICodePartsMatcher matcher, Task<TestsRootNode> testsTask)
        {
            _assembliesTask = assembliesTask;
            _matcher = matcher;
            _testsTask = testsTask;
        }

        public async Task<List<TestNodeAssembly>> SelectTests(List<string> testAssemblies = null)
        {
            var finder = new CoveringTestsFinder();
            List<CciModuleSource> modules = (await _assembliesTask)
                .Where(a => testAssemblies == null || testAssemblies.Select(Path.GetFileNameWithoutExtension)
                .Contains(a.Module.Name)).ToList();
            var coveringTask = finder.FindCoveringTests(modules, _matcher);

            var result = await TupleExtensions.WhenAll(Tuple.Create(coveringTask, _testsTask));
            var coveringTests = result.Item1;
            var testsRootNode = result.Item2;

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