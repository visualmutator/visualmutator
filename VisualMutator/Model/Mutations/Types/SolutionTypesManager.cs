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
            var namespaces = assemblies.Where(ass => ass.IsIncluded == true)
                .SelectMany(ass => ass.Children).Cast<TypeNamespaceNode>();

            return namespaces.Where(ns => ns.IsIncluded == true).SelectMany(GetIncluded).ToList();
        }
        private ICollection<TypeDefinition> GetIncluded(TypeNamespaceNode ns)
        {
            var list = new List<TypeDefinition>();
            foreach (GenericNode genericNode in ns.Children)
            {
                var nsNode = genericNode as TypeNamespaceNode;
                if (nsNode != null)
                {
                    list.AddRange(GetIncluded(nsNode));
                }
                else
                {
                    var typeNode = (TypeNode)genericNode;
                    if ((bool)typeNode.IsIncluded)
                    {
                        list.Add(typeNode.TypeDefinition);
                    }
                }
            }
            return list;
        }

        public IList<AssemblyNode> GetTypesFromAssemblies()
        {

            IEnumerable<string> paths = _visualStudio.GetProjectPaths();

            var ass = LoadAssemblies(paths);

            return BuildTree(ass);
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