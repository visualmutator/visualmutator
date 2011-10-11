using System.Collections;
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
       
        IList<AssemblyNode> GetTypesFromAssemblies();

        IList<TypeDefinition> GetIncludedTypes(IEnumerable<AssemblyNode> assemblies);

  
    }

    public class SolutionTypesManager : ITypesManager
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAssemblyReaderWriter _assemblyReaderWriter;

        private readonly IVisualStudioConnection _visualStudio;

   
        public SolutionTypesManager(
            IAssemblyReaderWriter assemblyReaderWriter,
            IVisualStudioConnection visualStudio)
        {
            _assemblyReaderWriter = assemblyReaderWriter;
            _visualStudio = visualStudio;
        }


        public IList<TypeDefinition> GetIncludedTypes(IEnumerable<AssemblyNode> assemblies)
        {
            return assemblies
                .SelectManyRecursive<GenericNode>(node => node.Children, node => node.IsIncluded ?? true, leafsOnly:true)
                .Cast<TypeNode>().Select(type=>type.TypeDefinition).ToList();
        }
       
        public IList<AssemblyNode> GetTypesFromAssemblies()
        {

            IEnumerable<string> paths = _visualStudio.GetProjectPaths();

            var ass = LoadAssemblies(paths);

            return BuildTree(ass);
        }

        private IEnumerable<AssemblyDefinition> LoadAssemblies(IEnumerable<string> projectsPaths)
        {
            var loadedAssemblies = new List<AssemblyDefinition>();
            foreach (var assembly in projectsPaths)
            {
                try
                {
                    loadedAssemblies.Add(_assemblyReaderWriter.ReadAssembly(assembly));
                }
                catch (FileNotFoundException e)
                {
                    _log.Info("ReadAssembly failed. ", e);
                }
            }
            return loadedAssemblies;
        }
        private IList<AssemblyNode> BuildTree(IEnumerable<AssemblyDefinition> loadedAssemblies)
        {
       
            var assemblyTreeNodes = new List<AssemblyNode>();
          
            var root = new FakeNode();

            foreach (var assembly in loadedAssemblies)
            {
                var assemblyNode = new AssemblyNode(assembly.Name.Name, assembly);
                GroupTypes(assemblyNode, "", assembly.MainModule.Types.Where(t => t.Name != "<Module>").ToList());

                root.Children.Add(assemblyNode);
                assemblyTreeNodes.Add(assemblyNode);
            }

            root.IsIncluded = true;

            return assemblyTreeNodes;
        }


        public void GroupTypes(GenericNode parent, string currentNamespace, ICollection<TypeDefinition> types)
        {
            var groupsByNamespaces = types
                .Where(t => t.Namespace != currentNamespace)
                .OrderBy(t => t.Namespace)
                .GroupBy(t => ExtractNextNamespacePart(t.Namespace, currentNamespace))
                .ToList();

            var leafTypes = types
                .Where(t => t.Namespace == currentNamespace)
                .OrderBy(t => t.Name)
                .ToList();

            // Maybe we can merge namespace nodes:
            if (currentNamespace != "" && groupsByNamespaces.Count == 1 && !leafTypes.Any())
            {
                var singleGroup = groupsByNamespaces.Single();
                parent.Name = ConcatNamespace(parent.Name, singleGroup.Key);
                GroupTypes(parent, ConcatNamespace(currentNamespace, singleGroup.Key), singleGroup.ToList());
            }
            else
            {
                foreach (var typesGroup in groupsByNamespaces)
                {
                    var node = new TypeNamespaceNode(parent, typesGroup.Key);
                    GroupTypes(node, ConcatNamespace(currentNamespace, typesGroup.Key), typesGroup.ToList());
                    parent.Children.Add(node);
                }

                foreach (TypeDefinition typeDefinition in leafTypes)
                {
                    parent.Children.Add(new TypeNode(parent, typeDefinition.Name, typeDefinition));
                  
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