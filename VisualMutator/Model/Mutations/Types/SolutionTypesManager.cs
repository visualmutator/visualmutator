namespace VisualMutator.Model.Mutations.Types
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using Exceptions;
    using Extensibility;
    using Infrastructure;
    using log4net;
    using Microsoft.Cci;
    using StoringMutants;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using MethodIdentifier = Model.MethodIdentifier;

    #endregion

    public interface ITypesManager
    {

        bool IsAssemblyLoadError { get; set; }


        IList<AssemblyNode> GetTypesFromAssemblies(IList<FilePathAbsolute> paths,
            ICodePartsMatcher constraints);

        MutationFilter CreateFilterBasedOnSelection(ICollection<AssemblyNode> assemblies);
    }
    public static class Helpers
    {
        public static string GetTypeFullName(this INamespaceTypeReference t)
        {
            var nsPart = TypeHelper.GetNamespaceName(t.ContainingUnitNamespace, NameFormattingOptions.None);
            var typePart = t.Name.Value + (t.MangleName ? "`"+t.GenericParameterCount : "");
            return nsPart + "." + typePart;
        }
    }
    public class SolutionTypesManager : ITypesManager
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICciModuleSource _moduleSource;

        public bool IsAssemblyLoadError { get; set; }
   
        public SolutionTypesManager(
            ICciModuleSource moduleSource)
        {
            _moduleSource = moduleSource;
        }

        public MutationFilter CreateFilterBasedOnSelection(ICollection<AssemblyNode> assemblies)
        {
            var methods = assemblies
                .SelectManyRecursive<CheckedNode>(node => node.Children, node => node.IsIncluded ?? true, leafsOnly: true)
                .OfType<MethodNode>().Select(type => type.MethodDefinition).ToList();
            return new MutationFilter(new List<TypeIdentifier>(), methods.Select(m => new Extensibility.MethodIdentifier(m)).ToList());
        }
      

        public IList<AssemblyNode> GetTypesFromAssemblies(IList<FilePathAbsolute> paths,
            ICodePartsMatcher constraints)
        {
            var matcher = constraints.Join(new ProperlyNamedMatcher());
            var loadedAssemblies = LoadAssemblies(paths, matcher);
            var root = new RootNode();
            root.Children.AddRange(loadedAssemblies);
            root.IsIncluded = true;

            return loadedAssemblies;
        }

       

        private IList<AssemblyNode> LoadAssemblies(IEnumerable<FilePathAbsolute> assembliesPaths, 
            ICodePartsMatcher matcher)
        {
            var assemblyTreeNodes = new List<AssemblyNode>();
            foreach (FilePathAbsolute assemblyPath in assembliesPaths)
            {
                try
                {
                    IModule module = _moduleSource.AppendFromFile((string)assemblyPath);
                    if(module.StrongNameSigned)
                    {
                       // throw new StrongNameSignedAssemblyException();
                    }

                    var assemblyNode = new AssemblyNode(module.Name.Value, module);

                    System.Action<CheckedNode, ICollection<INamedTypeDefinition>> typeNodeCreator = (parent, leafTypes) =>
                    {
                        foreach (INamedTypeDefinition typeDefinition in leafTypes)
                        {
                            if (matcher.Matches(typeDefinition))
                            {
                                var type = new TypeNode(parent, typeDefinition.Name.Value);
                                foreach (var method in typeDefinition.Methods)
                                {
                                    if (matcher.Matches(method))
                                    {
                                        type.Children.Add(new MethodNode(type, method.Name.Value, method, false));
                                    }
                                }
                                parent.Children.Add(type);
                            }
                        }
                    };
                    Func<INamedTypeDefinition, string> namespaceExtractor = typeDef =>
                        TypeHelper.GetDefiningNamespace(typeDef).Name.Value;
                    

                    NamespaceGrouper<INamespaceTypeDefinition, CheckedNode>.
                        GroupTypes(assemblyNode, 
                            namespaceExtractor, 
                            (parent, name) => new TypeNamespaceNode(parent, name), 
                            typeNodeCreator,
                                module.GetAllTypes().ToList());

                    
                    assemblyTreeNodes.Add(assemblyNode);

                    //remove empty amespaces. 
                    //TODO to refactor...
                    List<CheckedNode> checkedNodes = assemblyTreeNodes.SelectMany(a => a.Children).ToList();
                    foreach (TypeNamespaceNode node in checkedNodes)
                    {
                        RemoveFromParentIfEmpty(node);
                    }

                }
                catch (AssemblyReadException e)
                {
                    _log.Error("ReadAssembly failed. ", e);
                    IsAssemblyLoadError = true;
                }
                catch (Exception e)
                {
                    _log.Error("ReadAssembly failed. ", e);
                    IsAssemblyLoadError = true;
                }
            } 
            return assemblyTreeNodes;
        }
        public void RemoveFromParentIfEmpty(TypeNamespaceNode node)
        {
            while(node.Children.OfType<TypeNamespaceNode>().Any())
            {
                TypeNamespaceNode typeNamespaceNode = node.Children.OfType<TypeNamespaceNode>().First();
                RemoveFromParentIfEmpty(typeNamespaceNode);
            }
            if(!node.Children.Any())
            {
                node.Parent.Children.Remove(node);
                node.Parent = null;
            }
        }
        //TODO: nessessary?
        private static IEnumerable<INamespaceTypeDefinition> ChooseTypes(IModule module)
        {
            return module.GetAllTypes()
                .OfType<INamespaceTypeDefinition>()
                .Where(t => t.Name.Value != "<Module>")
                .Where(t => !t.Name.Value.StartsWith("<>"));

        }

        class ProperlyNamedMatcher : CodePartsMatcher
        {
            public override bool Matches(IMethodReference method)
            {
                return true;
            }

            public override bool Matches(ITypeReference typeReference)
            {
                INamedTypeReference named = typeReference as INamedTypeReference;
                return named != null
                       && named.Name.Value != "<Module>"
                       && !named.Name.Value.StartsWith("<>");
            }
        }

    }
}