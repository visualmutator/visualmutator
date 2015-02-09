namespace VisualMutator.Model
{
    using System.Linq;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    public static class TreeUtils
    {



        public static void ExpandLoneNodes(CheckedNode tests)
        {
            var allTests = tests.Children
                .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>(),
                    n => n.IsIncluded == null || n.IsIncluded == true)
                .Cast<IExpandableNode>();
            foreach (var node in allTests)
            {
                node.IsExpanded = true;
            }
        }
    }
}