namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Mutations;
    using Mutations.MutantsTree;
    using TestsTree;

    public class TestResultTreeCreator
    {
        public TestResultTreeCreator()
        {
        }

        public IEnumerable<TestNodeNamespace> CreateMutantTestTree(List<TmpTestNodeMethod> nodeMethods)
        {

            var list = nodeMethods.GroupBy(n => ExtractTypeName(n.Name)).ToList();

            TestsRootNode root = new TestsRootNode();
            var u = new Utilss();
            u.GroupTypes<IGrouping<string, TmpTestNodeMethod>, TestTreeNode>(
                root, n => ExtractTypeName(n.Key),
                (parent, name) => new TestNodeNamespace(parent, name),
                (parent, collection) =>
                {
                    foreach (var grouping in collection)
                    {
                        var testNodeClass = new TestNodeClass(ExtractName(grouping.Key));
                        testNodeClass.Parent = parent;
                        parent.Children.Add(testNodeClass);
                        foreach (var tmpMethod in grouping)
                        {
                            var testNodeMethod = new TestNodeMethod(testNodeClass, ExtractName(tmpMethod.Name));
                            testNodeMethod.State = tmpMethod.State;
                            testNodeMethod.Message = tmpMethod.Message;
                            testNodeClass.Children.Add(testNodeMethod);
                        }
                    }
                }, list);
            return root.Children.Cast<TestNodeNamespace>();
        }

        private string ExtractTypeName(string name)
        {
            return name.Substring(0, name.LastIndexOf("."));
        }

        private string ExtractName(string name)
        {
            int i = name.LastIndexOf(".") + 1;
            int len = name.Length - i;
            return name.Substring(i, len);
        }
    }
}