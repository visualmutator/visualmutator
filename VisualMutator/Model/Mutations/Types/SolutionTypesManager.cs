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
    using Microsoft.Cci;
    using Mono.Cecil;

    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.CheckboxedTree;
    using VisualMutator.Model.Exceptions;

    using log4net;

    #endregion

    public interface ITypesManager
    {
       
        IList<AssemblyNode> GetTypesFromAssemblies();

        LoadedTypes GetIncludedTypes(IEnumerable<AssemblyNode> assemblies);

        bool IsAssemblyLoadError { get; set; }

        IEnumerable<DirectoryPathAbsolute> ProjectPaths { get; }
    }

    public class SolutionTypesManager : ITypesManager
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICommonCompilerAssemblies _assemblyReaderWriter;

        private readonly IVisualStudioConnection _visualStudio;

        private readonly IEnumerable<DirectoryPathAbsolute> _projectPaths;

        public IEnumerable<DirectoryPathAbsolute> ProjectPaths
        {
            get
            {
                return _projectPaths;
            }
        }

        public bool IsAssemblyLoadError { get; set; }
   
        public SolutionTypesManager(
            ICommonCompilerAssemblies assemblyReaderWriter,
            IVisualStudioConnection visualStudio)
        {
            _assemblyReaderWriter = assemblyReaderWriter;
            _visualStudio = visualStudio;

            _projectPaths = _visualStudio.GetProjectPaths();
        }


        public LoadedTypes GetIncludedTypes(IEnumerable<AssemblyNode> assemblies)
        {
            var types = assemblies
                .SelectManyRecursive<NormalNode>(node => node.Children, node => node.IsIncluded ?? true, leafsOnly:true)
                .Cast<TypeNode>().Select(type=>type.TypeDefinition).ToList();
            return new LoadedTypes(types);
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
                    IModule module = _assemblyReaderWriter.AppendFromFile((string)assemblyPath);
                   
                    var assemblyNode = new AssemblyNode(module.Name.Value, module);
                    assemblyNode.AssemblyPath = assemblyPath;

                    GroupTypes(assemblyNode, "", ChooseTypes(module).ToList());

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
        //TODO: nessessary?
        private static IEnumerable<INamespaceTypeDefinition> ChooseTypes(IModule module)
        {
            return module.GetAllTypes()
                .OfType<INamespaceTypeDefinition>()
                .Where(t => t.Name.Value != "<Module>")
                .Where(t => !t.Name.Value.StartsWith("<>"));

        }

        public void GroupTypes(NormalNode parent, string currentNamespace, ICollection<INamespaceTypeDefinition> types)
        {
            var groupsByNamespaces = types
                .Where(t => t.ContainingNamespace.Name.Value != currentNamespace)
                .OrderBy(t => t.ContainingNamespace.Name.Value)
                .GroupBy(t => ExtractNextNamespacePart(t.ContainingNamespace.Name.Value, currentNamespace))
                .ToList();

            var leafTypes = types
                .Where(t => t.ContainingNamespace.Name.Value == currentNamespace)
                .OrderBy(t => t.Name.Value)
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

                foreach (INamespaceTypeDefinition typeDefinition in leafTypes)
                {
                    parent.Children.Add(new TypeNode(parent, typeDefinition.Name.Value, typeDefinition));
                  
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