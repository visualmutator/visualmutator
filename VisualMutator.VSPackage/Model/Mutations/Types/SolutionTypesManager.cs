namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    #endregion

    public interface ITypesManager
    {
        IEnumerable<AssemblyNode> AssemblyTreeNodes { get; }

        IEnumerable<AssemblyNode> BuildTypesTree(IEnumerable<string> projectsPaths);

        IEnumerable<TypeDefinition> GetIncludedTypes();

        IEnumerable<AssemblyDefinition> GetLoadedAssemblies();
    }

    public class SolutionTypesManager : ITypesManager
    {
        private readonly IAssemblyReaderWriter _assemblyReaderWriter;

        private IList<AssemblyNode> _assemblyTreeNodes;

        private IEnumerable<AssemblyDefinition> _loadedAssemblies;

        private IList<TypeNode> _types;

        public SolutionTypesManager(IAssemblyReaderWriter assemblyReaderWriter)
        {
            _assemblyReaderWriter = assemblyReaderWriter;
        }

        public IEnumerable<AssemblyNode> AssemblyTreeNodes
        {
            get
            {
                return _assemblyTreeNodes;
            }
        }

        public IEnumerable<AssemblyDefinition> GetLoadedAssemblies()
        {
            return _loadedAssemblies;
        }

        public IEnumerable<TypeDefinition> GetIncludedTypes()
        {
            return _types.Where(t => (bool)t.IsIncluded).Select(t => t.TypeDefinition);
        }

        public IEnumerable<AssemblyNode> BuildTypesTree(IEnumerable<string> projectsPaths)
        {
            _loadedAssemblies = projectsPaths.Select(p => _assemblyReaderWriter.ReadAssembly(p));

            var typesGroups = _loadedAssemblies.SelectMany(ad => ad.MainModule.Types)
                .Where(t => t.Name != "<Module>").GroupBy(t => t.Module.Assembly.Name.Name);

            _assemblyTreeNodes = new List<AssemblyNode>();
            _types = new List<TypeNode>();
            var root = new FakeNode();

            foreach (var types in typesGroups)
            {
                var assemblyNode = new AssemblyNode(types.Key);
                GroupTypes(assemblyNode, "", types);

                root.Children.Add(assemblyNode);
                _assemblyTreeNodes.Add(assemblyNode);
            }

            root.IsIncluded = true;

            return _assemblyTreeNodes;
        }
     
        public void GroupTypes(GenericNode parent,
                               string currentNamespace, IEnumerable<TypeDefinition> types)
        {
            var groupsByNamespace = types.Where(t => t.Namespace != currentNamespace &&
                                                     t.Namespace.StartsWith(currentNamespace))
                .OrderBy(t => t.Namespace)
                .GroupBy(t => ExtractNextNamespacePart(t.Namespace, currentNamespace));

            var leafTypes = types.Where(t => t.Namespace == currentNamespace)
                .OrderBy(t => t.Name);

            if (currentNamespace != "" && groupsByNamespace.Count() == 1 && !leafTypes.Any())
            {
                var singleGroup = groupsByNamespace.Single();
                parent.Name = ConcatNamespace(parent.Name, singleGroup.Key);
                GroupTypes(parent, ConcatNamespace(currentNamespace, singleGroup.Key), singleGroup);
            }
            else
            {
                foreach (var typesGroup in groupsByNamespace)
                {
                    var node = new TypeNamespaceNode(parent, typesGroup.Key);
                    GroupTypes(node, ConcatNamespace(currentNamespace, typesGroup.Key), typesGroup);
                    parent.Children.Add(node);
                }

                foreach (TypeDefinition typeDefinition in leafTypes)
                {
                    var typeNode = new TypeNode(parent, typeDefinition.Name, typeDefinition);
                    parent.Children.Add(typeNode);
                    _types.Add(typeNode);
                }
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