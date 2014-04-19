namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Types;
    using UsefulTools.CheckboxedTree;
    public class Utilss
    {
        public void GroupTypes<T, Node>(Node parent,
            Func<T, string> namespaceExtractor,
            Func<Node, string, Node> namespaceNodeCreator,
            Action<Node, ICollection<T>> typeNodeCreator,
            ICollection<T> types) where Node : CheckedNode
        {
            var ng = new NamespaceGrouper<T, Node>(namespaceExtractor, namespaceNodeCreator, typeNodeCreator);
            ng.GroupTypes2(parent, "", types);
        }
    }
    public class NamespaceGrouper<T, Node> where Node : CheckedNode
    {
        private readonly Func<T, string> _namespaceExtractor;
        private readonly Func<Node, string, Node> _namespaceNodeCreator;
        private readonly Action<Node, ICollection<T>> _typeNodeCreator;

        public NamespaceGrouper(
            Func<T, string> namespaceExtractor,
            Func<Node, string, Node> namespaceNodeCreator,
            Action<Node, ICollection<T>> typeNodeCreator)
        {
            _namespaceExtractor = namespaceExtractor;
            _namespaceNodeCreator = namespaceNodeCreator;
            _typeNodeCreator = typeNodeCreator;
        }

        public static void GroupTypes<T1, Node1>(Node1 parent,
            Func<T1, string> namespaceExtractor,
            Func<Node1, string, Node1> namespaceNodeCreator,
            Action<Node1, ICollection<T1>> typeNodeCreator,
            ICollection<T1> types) where Node1 : CheckedNode
        {
            var ng = new NamespaceGrouper<T1, Node1>(namespaceExtractor, namespaceNodeCreator, typeNodeCreator);
            ng.GroupTypes2(parent, "", types);
        }


        public void GroupTypes2(Node parent, string currentNamePart, ICollection<T> types)
        {
            List<IGrouping<string, T>> groupsByNamespaces = types
                .Where(t => _namespaceExtractor(t) != currentNamePart)
                .OrderBy(_namespaceExtractor)
                .GroupBy(t => ExtractNextNamespacePart(_namespaceExtractor(t), currentNamePart))
                .ToList();

            List<T> leafTypes = types
                .Where(t => _namespaceExtractor(t) == currentNamePart)
                .ToList();

            // Maybe we can merge namespace nodes:
            if (currentNamePart != "" && groupsByNamespaces.Count == 1 && !leafTypes.Any())
            {
                var singleGroup = groupsByNamespaces.Single();
                parent.Name = ConcatNamespace(parent.Name, singleGroup.Key);
                string nextPart = ConcatNamespace(currentNamePart, singleGroup.Key);
                GroupTypes2(parent, nextPart, singleGroup.ToList());
            }
            else
            {
                foreach (var typesGroup in groupsByNamespaces)
                {
                    var node = _namespaceNodeCreator(parent, typesGroup.Key);
                    string nextPart = ConcatNamespace(currentNamePart, typesGroup.Key);
                    GroupTypes2(node, nextPart, typesGroup.ToList());
                    parent.Children.Add(node);
                }

                _typeNodeCreator(parent, leafTypes);
            }
        }



        public string ConcatNamespace(string one, string two)
        {
            return one == "" ? two : one + "." + two;
        }

        public string ExtractNextNamespacePart(string extractFrom, string namespaceName)
        {
            if (!extractFrom.StartsWith(namespaceName))
            {
                throw new ArgumentException("extractFrom");
            }

            if (namespaceName != "")
            {
                extractFrom = extractFrom.Remove(
                    0, namespaceName.Length + 1);
            }

            int index = extractFrom.IndexOf('.');
            return index != -1 ? extractFrom.Remove(extractFrom.IndexOf('.')) : extractFrom;
        }
    }
}