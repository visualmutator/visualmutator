namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TestsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;

    public class TestsSelector
    {

        public SelectedTests GetIncludedTests(IEnumerable<TestNodeAssembly> assemblyNodes)
        {
            ICollection<TestId> selected = assemblyNodes
                .SelectManyRecursive<CheckedNode>(node => node.Children, node => node.IsIncluded ?? true, leafsOnly: true)
                .Cast<TestNodeMethod>().Select(m => m.TestId).ToList();

            return new SelectedTests();
        }

        public string NameExtractor(TestTreeNode node)
        {
            return Switch.Into<string>().FromTypeOf(node)
                .Case<TestNodeNamespace>(n => n.Name)
                .Case<TestNodeClass>(n => n.FullName)
                .Case<TestNodeMethod>(n => n.TestId.ToString())
                .GetResult();
        }

        public string CreateMinimalTestsInfo(IEnumerable<TestNodeNamespace> namespaces)
        {
            return string.Join(" ", namespaces
                .Select(n => MinimalTreeId((TestTreeNode)n, NameExtractor, a => a.Children.Cast<TestTreeNode>())));
        }

        public string MinimalTreeId<Node>(Node node,
            Func<Node, string> nameExtractor, Func<Node, IEnumerable<Node>> childExtractor) where Node : CheckedNode
        {
            if(node.IsIncluded.HasValue)
            {
                if (node.IsIncluded.Value)
                {
                    return nameExtractor(node);
                }
                else return "";
            }
            else
            {
                var strings = childExtractor(node)
                    .Select(n => MinimalTreeId(n, nameExtractor, childExtractor));
                return string.Join(" ", strings);
            }

        }
    }
}