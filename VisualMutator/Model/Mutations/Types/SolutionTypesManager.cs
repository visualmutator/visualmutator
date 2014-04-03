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
            MethodIdentifier constraints);

        MutationFilter CreateFilterBasedOnSelection(IEnumerable<AssemblyNode> assemblies);
        List<MethodIdentifier> FindCoveredTests(IList<AssemblyNode> loadedAssemblies, MethodIdentifier constraints);
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

        public MutationFilter CreateFilterBasedOnSelection(IEnumerable<AssemblyNode> assemblies)
        {
            var methods = assemblies
                .SelectManyRecursive<CheckedNode>(node => node.Children, node => node.IsIncluded ?? true, leafsOnly: true)
                .OfType<MethodNode>().Select(type => type.MethodDefinition).ToList();
            return new MutationFilter(new List<TypeIdentifier>(), methods.Select(m => new Extensibility.MethodIdentifier(m)).ToList());
        }
      

        public IList<AssemblyNode> GetTypesFromAssemblies(IList<FilePathAbsolute> paths,
            MethodIdentifier constraints)
        {

            var loadedAssemblies = LoadAssemblies(paths, constraints);
            var root = new RootNode();
            root.Children.AddRange(loadedAssemblies);
            root.IsIncluded = true;

            return loadedAssemblies;
        }

        public List<MethodIdentifier> FindCoveredTests(IList<AssemblyNode> loadedAssemblies, MethodIdentifier constraints)
        {
            return loadedAssemblies.Select(a => a.AssemblyDefinition).SelectMany(m =>
            {
                _log.Debug("Scanning "+m.Name.Value+" for selected covering tests. ");
                var visitor = new CoveringTestsVisitor(constraints);

                var traverser = new CodeTraverser {PreorderVisitor = visitor};

                traverser.Traverse(m);
                _log.Debug("Finished scanning. Found " + visitor.FoundTests.Count);
                if (visitor.IsChoiceError)
                {
                    throw new TestWasSelectedToMutateException();
                }
                return visitor.FoundTests;
            }).ToList();
            
        }

        private IList<AssemblyNode> LoadAssemblies(IEnumerable<FilePathAbsolute> assembliesPaths, 
            MethodIdentifier constraints = null)
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

                    System.Action<CheckedNode, ICollection<INamespaceTypeDefinition>> typeNodeCreator = (parent, leafTypes) =>
                    {
                        foreach (INamespaceTypeDefinition typeDefinition in leafTypes)
                        {
                            var type = new TypeNode(parent, typeDefinition.Name.Value);
                            foreach (var method in typeDefinition.Methods)
                            {
                                if (constraints == null || constraints.MethodName == method.Name.Value)
                                {
                                    type.Children.Add(new MethodNode(type, method.Name.Value, method, false));
                                }
                            }

                            parent.Children.Add(type);

                        }
                    };
                    Func<INamespaceTypeDefinition, string> namespaceExtractor = typeDef => 
                        typeDef.ContainingUnitNamespace.Name.Value;

                    NamespaceGrouper<INamespaceTypeDefinition, CheckedNode>.
                        GroupTypes(assemblyNode, namespaceExtractor, 
                        (parent, name) => new TypeNamespaceNode(parent, name), typeNodeCreator,
                            ChooseTypes(module, constraints).ToList());

                    
                    assemblyTreeNodes.Add(assemblyNode);

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
    
        //TODO: nessessary?
        private static IEnumerable<INamespaceTypeDefinition> ChooseTypes(IModule module, MethodIdentifier constraints = null)
        {
            return module.GetAllTypes()
                .OfType<INamespaceTypeDefinition>()
                .Where(t => constraints==null || t.GetTypeFullName() == constraints.ClassName)
                .Where(t => t.Name.Value != "<Module>")
                .Where(t => !t.Name.Value.StartsWith("<>"));

        }
    }
}