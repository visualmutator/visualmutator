namespace VisualMutator.Model.Mutations.Types
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Mono.Cecil;

    using VisualMutator.Infrastructure;

    using log4net;

    #endregion

    public interface ITypesManager
    {
        IEnumerable<AssemblyNode> AssemblyTreeNodes { get; }

        IEnumerable<AssemblyNode> GetTypesFromAssemblies(IEnumerable<string> projectsPaths);

        ICollection<TypeDefinition> GetIncludedTypes();

        IEnumerable<AssemblyDefinition> GetLoadedAssemblies();
    }

    public class SolutionTypesManager : ITypesManager
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAssemblyReaderWriter _assemblyReaderWriter;

        private IList<AssemblyNode> _assemblyTreeNodes;

        private ICollection<AssemblyDefinition> _loadedAssemblies;

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

        public ICollection<TypeDefinition> GetIncludedTypes()
        {
            return _types.Where(t => (bool)t.IsIncluded).Select(t => t.TypeDefinition).ToList();
        }

        public IEnumerable<AssemblyNode> GetTypesFromAssemblies(IEnumerable<string> projectsPaths)
        {
            LoadAssemblies(projectsPaths);

            return BuildTree();
        }

        private IEnumerable<AssemblyNode> BuildTree()
        {
            var typesGroups = _loadedAssemblies.SelectMany(ad => ad.MainModule.Types)
                .Where(t => t.Name != "<Module>").GroupBy(t => t.Module.Assembly.Name.Name);

            _assemblyTreeNodes = new List<AssemblyNode>();
            _types = new List<TypeNode>();
            var root = new FakeNode();

            foreach (var types in typesGroups)
            {
                var assemblyNode = new AssemblyNode(types.Key);
                GroupTypes(assemblyNode, "", types.ToList());

                root.Children.Add(assemblyNode);
                _assemblyTreeNodes.Add(assemblyNode);
            }

            root.IsIncluded = true;

            return _assemblyTreeNodes;
        }

        private void LoadAssemblies(IEnumerable<string> projectsPaths)
        {
            _loadedAssemblies = new List<AssemblyDefinition>();
            foreach (var assembly in projectsPaths)
            {
                try
                {
                    _loadedAssemblies.Add(_assemblyReaderWriter.ReadAssembly(assembly));
                }
                catch (FileNotFoundException e)
                {
                    _log.Info("ReadAssembly failed. ", e);
                }
            }
        }

        public void GroupTypes(GenericNode parent,
                               string currentNamespace, ICollection<TypeDefinition> types)
        {
            var groupsByNamespace = types.Where(t => t.Namespace != currentNamespace &&
                                                     t.Namespace.StartsWith(currentNamespace))
                .OrderBy(t => t.Namespace)
                .GroupBy(t => ExtractNextNamespacePart(t.Namespace, currentNamespace)).ToList();

            var leafTypes = types.Where(t => t.Namespace == currentNamespace)
                .OrderBy(t => t.Name);

            if (currentNamespace != "" && groupsByNamespace.Count() == 1 && !leafTypes.Any())
            {
                var singleGroup = groupsByNamespace.Single();
                parent.Name = ConcatNamespace(parent.Name, singleGroup.Key);
                GroupTypes(parent, ConcatNamespace(currentNamespace, singleGroup.Key), singleGroup.ToList());
            }
            else
            {
                foreach (var typesGroup in groupsByNamespace)
                {
                    var node = new TypeNamespaceNode(parent, typesGroup.Key);
                    GroupTypes(node, ConcatNamespace(currentNamespace, typesGroup.Key), typesGroup.ToList());
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