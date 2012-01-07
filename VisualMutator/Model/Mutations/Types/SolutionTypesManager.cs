using System.Collections;
namespace VisualMutator.Model.Mutations.Types
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Paths;

    using Mono.Cecil;

    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.CheckboxedTree;
    using VisualMutator.Model.Exceptions;

    using log4net;

    #endregion

    public interface ITypesManager
    {
       
        IList<AssemblyNode> GetTypesFromAssemblies();

        IList<TypeDefinition> GetIncludedTypes(IEnumerable<AssemblyNode> assemblies);

        bool IsAssemblyLoadError { get; set; }

        IEnumerable<DirectoryPathAbsolute> ProjectPaths { get; }
    }

    public class SolutionTypesManager : ITypesManager
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAssemblyReaderWriter _assemblyReaderWriter;

        private readonly IVisualStudioConnection _visualStudio;

        private IEnumerable<DirectoryPathAbsolute> _projectPaths;

        public IEnumerable<DirectoryPathAbsolute> ProjectPaths
        {
            get
            {
                return _projectPaths;
            }
        }

        public bool IsAssemblyLoadError { get; set; }
   
        public SolutionTypesManager(
            IAssemblyReaderWriter assemblyReaderWriter,
            IVisualStudioConnection visualStudio)
        {
            _assemblyReaderWriter = assemblyReaderWriter;
            _visualStudio = visualStudio;

            _projectPaths = _visualStudio.GetProjectPaths();
        }


        public IList<TypeDefinition> GetIncludedTypes(IEnumerable<AssemblyNode> assemblies)
        {
            return assemblies
                .SelectManyRecursive<NormalNode>(node => node.Children, node => node.IsIncluded ?? true, leafsOnly:true)
                .Cast<TypeNode>().Select(type=>type.TypeDefinition).ToList();
        }
       
        public IList<AssemblyNode> GetTypesFromAssemblies()
        {

            

            var root = new FakeNode();
            var loadedAssemblies = LoadAssemblies(root, _visualStudio.GetProjectAssemblyPaths());
            root.IsIncluded = true;

            return loadedAssemblies;
        }

        private IList<AssemblyNode> LoadAssemblies(FakeNode root, IEnumerable<FilePathAbsolute> assembliesPaths)
        {
            var assemblyTreeNodes = new List<AssemblyNode>();
            foreach (FilePathAbsolute assemblyPath in assembliesPaths)
            {
                
                try
                {
                    var assemblyDefinition = _assemblyReaderWriter.ReadAssembly((string)assemblyPath);
                   
                    var assemblyNode = new AssemblyNode(assemblyDefinition.Name.Name, assemblyDefinition);
                    assemblyNode.AssemblyPath = assemblyPath;

                    GroupTypes(assemblyNode, "", ChooseTypes(assemblyDefinition).ToList());

                    root.Children.Add(assemblyNode);
                    assemblyTreeNodes.Add(assemblyNode);

                }
                catch (AssemblyReadException e)
                {
                    _log.Info("ReadAssembly failed. ", e);
                    IsAssemblyLoadError = true;
                }
            }
            return assemblyTreeNodes;
        }
  
        private static IEnumerable<TypeDefinition> ChooseTypes(AssemblyDefinition assembly)
        {
            return assembly.MainModule.Types
                .Where(t => t.Name != "<Module>")
                .Where(t => !t.Name.StartsWith("<>"));
        }

        public void GroupTypes(NormalNode parent, string currentNamespace, ICollection<TypeDefinition> types)
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