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
    using Microsoft.Cci.Immutable;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using GenericTypeInstanceReference = Microsoft.Cci.MutableCodeModel.GenericTypeInstanceReference;

    #endregion

    public interface ITypesManager
    {

        IList<AssemblyNode> GetTypesFromAssemblies(IList<FilePathAbsolute> paths);

     
        bool IsAssemblyLoadError { get; set; }


        IList<AssemblyNode> GetTypesFromAssemblies(IList<FilePathAbsolute> paths,
            ClassAndMethod constraints, out List<ClassAndMethod> coveredTests);

        MutationFilter CreateFilterBasedOnSelection(IEnumerable<AssemblyNode> assemblies);
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
        private readonly IModuleSource _moduleSource;



  

        public bool IsAssemblyLoadError { get; set; }
   
        public SolutionTypesManager(
            IModuleSource moduleSource)
        {
            _moduleSource = moduleSource;

        }

        public MutationFilter CreateFilterBasedOnSelection(IEnumerable<AssemblyNode> assemblies)
        {
            var methods = assemblies
                .SelectManyRecursive<CheckedNode>(node => node.Children, node => node.IsIncluded ?? true, leafsOnly: true)
                .OfType<MethodNode>().Select(type => type.MethodDefinition).ToList();
            return new MutationFilter(new List<TypeIdentifier>(), methods.Select(m => new MethodIdentifier(m)).ToList());
        }
       

     
       
        public IList<AssemblyNode> GetTypesFromAssemblies(IList<FilePathAbsolute> paths)
        {

            var loadedAssemblies = LoadAssemblies(paths);
            var root = new RootNode();
            root.Children.AddRange(loadedAssemblies);
            root.IsIncluded = true;

            return loadedAssemblies;
        }
        public IList<AssemblyNode> GetTypesFromAssemblies(IList<FilePathAbsolute> paths,
            ClassAndMethod constraints, out List<ClassAndMethod> coveredTests)
        {

            var loadedAssemblies = LoadAssemblies(paths, constraints);
            var root = new RootNode();
            root.Children.AddRange(loadedAssemblies);
            root.IsIncluded = true;

            coveredTests = FindCoveredTests(loadedAssemblies.Select(a => a.AssemblyDefinition).ToList(), 
                constraints);

            return loadedAssemblies;
        }
        class V : CodeVisitor
        {
            private readonly ClassAndMethod _constraints;
            private readonly HashSet<IMethodDefinition> _foundTests;

            public HashSet<IMethodDefinition> FoundTests
            {
                get { return _foundTests; }
            }

            public V(ClassAndMethod constraints)
            {
                _constraints = constraints;
                _foundTests = new HashSet<IMethodDefinition>();
            }

            public override void Visit(IMethodDefinition method)
            {
                CurrentTestMethod = method.Attributes.Any(a =>
                {
                    var t = (INamespaceTypeReference) a.Type;
                    var typeName = t.GetTypeFullName();
                   bool isTest = typeName
                    == "NUnit.Framework.TestAttribute";
                   
                    return isTest;
                }) ? method : null;

                var def = method.ContainingTypeDefinition as INamespaceTypeDefinition;
               // def.Gen
                if (def != null && def.GetTypeFullName() == _constraints.ClassName
                    && method.Name.Value == _constraints.MethodName)
                {
                    if (CurrentTestMethod != null)
                    {
                        IsChoiceError = true;
                    }
                }
            }

            public bool IsChoiceError { get; set; }

            public IMethodDefinition CurrentTestMethod { get; set; }

            public override void Visit(IMethodCall methodCall)
            {
                base.Visit(methodCall);
                var conainingType = methodCall.MethodToCall.ContainingType as INamespaceTypeReference;
                var genericInstance = methodCall.MethodToCall.ContainingType as GenericTypeInstanceReference;
                if(genericInstance != null)
                {
                    conainingType = genericInstance.ResolvedType
                        .CastTo<GenericTypeInstance>().GenericType as INamespaceTypeReference;
                }
                if (CurrentTestMethod != null && conainingType != null )
                {
                    var res = conainingType.ResolvedType;
                    
                    if (res.GetTypeFullName() == _constraints.ClassName 
                           && _constraints.MethodName == methodCall.MethodToCall.Name.Value)
                    {
                        _foundTests.Add(CurrentTestMethod);
                    }
                }
            }
        }
        private List<ClassAndMethod> FindCoveredTests(IList<IModule> loadedAssemblies, ClassAndMethod constraints)
        {
            return (from m in loadedAssemblies.SelectMany(m =>
            {
                var visitor = new V(constraints);

                var traverser = new CodeTraverser {PreorderVisitor = visitor};

                traverser.Traverse(m);
                if(visitor.IsChoiceError)
                {
                    throw new TestWasSelectedToMutateException();
                }
                return visitor.FoundTests;
            })
                let t = m.ContainingTypeDefinition as INamespaceTypeDefinition
                where t != null
                select new ClassAndMethod(
                    TypeHelper.GetNamespaceName(t.ContainingUnitNamespace, NameFormattingOptions.None)
                     + "." + t.Name.Value, m.Name.Value)).ToList();
            //.Where(m => m.ContainingTypeDefinition is INamespaceTypeDefinition)
            //   .Select(m => m.ContainingTypeDefinition.CastTo<INamespaceTypeDefinition>().Name.Value+"."+
            //   m.Name.Value).ToList();


/*
            loadedAssemblies.Select(a => a.AssemblyDefinition).SelectMany(m => m.GetAllTypes())
                .AsParallel().SelectMany(t => t.Methods).
            foreach (var loadedAssembly in loadedAssemblies.Select(a => a.AssemblyDefinition))
            {
                
            }*/
        }

        private IList<AssemblyNode> LoadAssemblies(IEnumerable<FilePathAbsolute> assembliesPaths, 
            ClassAndMethod constraints = null)
        {
            var assemblyTreeNodes = new List<AssemblyNode>();
            foreach (FilePathAbsolute assemblyPath in assembliesPaths)
            {
                try
                {
                    IModule module = _moduleSource.AppendFromFile((string)assemblyPath);
                   
                    var assemblyNode = new AssemblyNode(module.Name.Value, module);

                    GroupTypes(assemblyNode, "", ChooseTypes(module, constraints).ToList(), constraints);

                    
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
        private static IEnumerable<INamespaceTypeDefinition> ChooseTypes(IModule module, ClassAndMethod constraints = null)
        {
            return module.GetAllTypes()
                .OfType<INamespaceTypeDefinition>()
                .Where(t => constraints==null || t.GetTypeFullName() == constraints.ClassName)
                .Where(t => t.Name.Value != "<Module>")
                .Where(t => !t.Name.Value.StartsWith("<>"));

        }

        public void GroupTypes(CheckedNode parent, string currentNamespace, ICollection<INamespaceTypeDefinition> types,
            ClassAndMethod constraints = null)
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
                GroupTypes(parent, ConcatNamespace(currentNamespace, singleGroup.Key), singleGroup.ToList(), constraints);
            }
            else
            {
                foreach (var typesGroup in groupsByNamespaces)
                {
                    var node = new TypeNamespaceNode(parent, typesGroup.Key);
                    GroupTypes(node, ConcatNamespace(currentNamespace, typesGroup.Key), typesGroup.ToList(), constraints);
                    parent.Children.Add(node);
                }

                foreach (INamespaceTypeDefinition typeDefinition in leafTypes)
                {
                    var type = new TypeNode(parent, typeDefinition.Name.Value, typeDefinition);
                    foreach (var method in typeDefinition.Methods)
                    {
                        if (constraints == null || constraints.MethodName == method.Name.Value)
                        {
                            type.Children.Add(new MethodNode(type, method.Name.Value, method));
                        }
                    }
                    parent.Children.Add(type);
                  
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