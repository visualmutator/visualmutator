namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Types;
    using UsefulTools.CheckboxedTree;

    public class NamespaceGrouper
    {
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
        public void GroupTypes2<T>(CheckedNode parent, string currentNamespace,
           Func<T, string> namespaceExtractor, Func<T, string> nameExtractor,
           System.Action<CheckedNode, List<T>> typeNodeCreator, ICollection<T> types)
        {
            var groupsByNamespaces = types
                .Where(t => namespaceExtractor(t) != currentNamespace)
                .OrderBy(namespaceExtractor)
                .GroupBy(t => ExtractNextNamespacePart(namespaceExtractor(t), currentNamespace))
                .ToList();

            var leafTypes = types
                .Where(t => namespaceExtractor(t) == currentNamespace)
                .OrderBy(nameExtractor)
                .ToList();

            // Maybe we can merge namespace nodes:
            if (currentNamespace != "" && groupsByNamespaces.Count == 1 && !leafTypes.Any())
            {
                var singleGroup = groupsByNamespaces.Single();
                parent.Name = ConcatNamespace(parent.Name, singleGroup.Key);
                GroupTypes2(parent, ConcatNamespace(currentNamespace, singleGroup.Key),
                    namespaceExtractor, nameExtractor, typeNodeCreator, singleGroup.ToList());
            }
            else
            {
                foreach (var typesGroup in groupsByNamespaces)
                {
                    var node = new TypeNamespaceNode(parent, typesGroup.Key);
                    GroupTypes2(node, ConcatNamespace(currentNamespace, typesGroup.Key),
                        namespaceExtractor, nameExtractor, typeNodeCreator, typesGroup.ToList());
                    parent.Children.Add(node);
                }

                typeNodeCreator(parent, leafTypes);
                
            }
        } 
    }
}