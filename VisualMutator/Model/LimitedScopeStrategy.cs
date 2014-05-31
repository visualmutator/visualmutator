namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.Cci;
    using Mutations;
    using Mutations.MutantsTree;
    using Mutations.Types;
    using StoringMutants;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    interface IMutationSessionStrategy
    {
         
    }

    public class LimitedScopeStrategy : IMutationSessionStrategy
    {
        private readonly MethodIdentifier _singleMethod;
        private CciMethodMatcher _matcher;

        public LimitedScopeStrategy(MethodIdentifier singleMethod)
        {
            _singleMethod = singleMethod;
            _matcher = new CciMethodMatcher(singleMethod);
        }


        public async Task<List<AssemblyNode>> BuildAssemblyTree(Task<CciModuleSource> assembliesTask,
          bool constrainedMutation, ICodePartsMatcher matcher)
        {
            var modules = await assembliesTask;
            var assemblies = CreateNodesFromAssemblies(modules, matcher)
                .Where(a => a.Children.Count > 0).ToList();

            var root = new CheckedNode("");
            root.Children.AddRange(assemblies);
            ExpandLoneNodes(root);

            if (assemblies.Count == 0)
            {
                throw new InvalidOperationException(UserMessages.ErrorNoFilesToMutate());
            }
            //  _reporting.LogError(UserMessages.ErrorNoFilesToMutate());
            return assemblies;
            //Events.OnNext(assemblies);
        }

        public IList<AssemblyNode> CreateNodesFromAssemblies(IModuleSource modules,
          ICodePartsMatcher constraints)
        {
            var matcher = constraints.Join(new SolutionTypesManager.ProperlyNamedMatcher());

            List<AssemblyNode> assemblyNodes = modules.Modules.Select(m => CreateAssemblyNode(m, matcher)).ToList();
            var root = new RootNode();
            root.Children.AddRange(assemblyNodes);
            root.IsIncluded = true;

            return assemblyNodes;
        }

        private void ExpandLoneNodes(CheckedNode tests)
        {
            var allTests = tests.Children
                .SelectManyRecursive(n => n.Children ?? new NotifyingCollection<CheckedNode>(),
                    n => n.IsIncluded == null || n.IsIncluded == true)
                .Cast<IExpandableNode>();
            foreach (var testNode in allTests)
            {
                testNode.IsExpanded = true;
            }
        }

        public AssemblyNode CreateAssemblyNode(IModuleInfo module,
            ICodePartsMatcher matcher)
        {
            var assemblyNode = new AssemblyNode(module.Name);

            System.Action<CheckedNode, ICollection<INamedTypeDefinition>> typeNodeCreator = (parent, leafTypes) =>
            {
                foreach (INamedTypeDefinition typeDefinition in leafTypes)
                {
                    // _log.Debug("For types: matching: ");
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
                        module.Module.GetAllTypes().ToList());


            //remove empty amespaces. 
            //TODO to refactor...
            List<TypeNamespaceNode> checkedNodes = assemblyNode.Children.OfType<TypeNamespaceNode>().ToList();
            foreach (TypeNamespaceNode node in checkedNodes)
            {
                RemoveFromParentIfEmpty(node);
            }
            return assemblyNode;
        }
        public void RemoveFromParentIfEmpty(MutationNode node)
        {
            var children = node.Children.ToList();
            while (children.OfType<TypeNamespaceNode>().Any())
            {
                TypeNamespaceNode typeNamespaceNode = node.Children.OfType<TypeNamespaceNode>().First();
                RemoveFromParentIfEmpty(typeNamespaceNode);
                children.Remove(typeNamespaceNode);
            }
            while (children.OfType<TypeNode>().Any())
            {
                TypeNode typeNamespaceNode = node.Children.OfType<TypeNode>().First();
                RemoveFromParentIfEmpty(typeNamespaceNode);
                children.Remove(typeNamespaceNode);
            }
            if (!node.Children.Any())
            {
                node.Parent.Children.Remove(node);
                node.Parent = null;
            }
        }

    }
}