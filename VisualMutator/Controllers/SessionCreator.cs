namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Model;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Model.Tests.TestsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    public class SessionCreator
    {
        private readonly ITypesManager _typesManager;
        private readonly IOperatorsManager _operatorsManager;
        private readonly CommonServices _svc;
        private readonly IMessageService _reporting;

        public SessionCreator(
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            CommonServices svc,
            IMessageService reporting)
        {
            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _svc = svc;
            _reporting = reporting;
            Events = new Subject<object>();
        }

        public Subject<object> Events { get; set; }

        public async Task<List<MethodIdentifier>> FindCoveringTests(Task<IModuleSource> assembliesTask, ICodePartsMatcher matcher)
        {
            var finder = new CoveringTestsFinder();
            IModuleSource modules = await assembliesTask;
            return await finder.FindCoveringTests(modules, matcher);
        }


        public async Task<OperatorPackagesRoot> GetOperators()
        {
            try
            {
                OperatorPackagesRoot root = await _operatorsManager.GetOperators();
                return root;
            }
            catch (Exception e)
            {
                _reporting.ShowError(e.ToString());
                throw;
            }
            //Events.OnNext(root);
            
        }

        public async Task<List<TestNodeAssembly>> BuildTestTree(Task<List<MethodIdentifier>> coveringTask, Task<object> testsTask, bool constrainedMutation)
        {

            var result = await Tuple.Create(coveringTask, testsTask).WhenAll();

            var coveringTests = result.Item1;
            var testsRootNode = (TestsRootNode)result.Item2;

            if (constrainedMutation)
            {
                SelectOnlyCoveredTests(testsRootNode, coveringTests);
            }

            if (_typesManager.IsAssemblyLoadError)
            {
                _reporting.ShowWarning(UserMessages.WarningAssemblyNotLoaded());
            }
            if (constrainedMutation)
            {
                ExpandLoneNodes(testsRootNode);
            }
            //Events.OnNext(testsRootNode.TestNodeAssemblies.ToList());
            return testsRootNode.TestNodeAssemblies.ToList();
        }

        public async Task<List<AssemblyNode>> BuildAssemblyTree(Task<IModuleSource> assembliesTask,
            bool constrainedMutation, ICodePartsMatcher matcher)
        {
            var modules = await assembliesTask;
            var assemblies = _typesManager.CreateNodesFromAssemblies(modules, matcher)
                .Where(a => a.Children.Count > 0).ToList();

            if (constrainedMutation)
            {
                var root = new CheckedNode("");
                root.Children.AddRange(assemblies);
                ExpandLoneNodes(root);
            }
            if(assemblies.Count == 0)
            {
                throw new InvalidOperationException(UserMessages.ErrorNoFilesToMutate());
            }
            //  _reporting.LogError(UserMessages.ErrorNoFilesToMutate());
            return assemblies;
            //Events.OnNext(assemblies);
        }

        private void ExpandLoneNodes(CheckedNode tests)
        {
            var allTests = tests.Children
                .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>(),
                    n => n.IsIncluded == null || n.IsIncluded == true)
                .Cast<IExpandableNode>();
            foreach (var testNode in allTests)
            {
                testNode.IsExpanded = true;
            }
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