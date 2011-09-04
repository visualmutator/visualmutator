namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    #endregion

    public interface ITypesManager
    {
        BetterObservableCollection<AssemblyNode> Assemblies { get; set; }

        void RefreshTypes(IEnumerable<string> projectsPaths);

        IEnumerable<TypeDefinition> GetIncludedTypes();

        IEnumerable<AssemblyDefinition> GetLoadedAssemblies();
    }

    public class SolutionTypesManager : ITypesManager
    {
        private IEnumerable<AssemblyDefinition> _loadedAssemblies;

        public SolutionTypesManager()
        {
            Assemblies = new BetterObservableCollection<AssemblyNode>();
            Types = new BetterObservableCollection<TypeNode>();
        }




        public BetterObservableCollection<AssemblyNode> Assemblies
        {
            get;
            set;
        }

        public BetterObservableCollection<TypeNode> Types
        {
            get;
            set;
        }

       


        public void RefreshTypes(IEnumerable<string> projectsPaths)
        {

            _loadedAssemblies = projectsPaths
                .Select(AssemblyDefinition.ReadAssembly);
            

       //    
            BuildTypesTree(_loadedAssemblies
            .ToDictionary(ad=>ad.Name.Name, 
            ad=>ad.MainModule.Types.Where(t => t.Name != "<Module>")));

            // TypeDefinition d = new TypeDefinition();

        }

        public IEnumerable<AssemblyDefinition> GetLoadedAssemblies()
        {
            return _loadedAssemblies;
        }

        public void BuildTypesTree(IDictionary<string, IEnumerable<TypeDefinition>> typesDictionary)
        {
            Assemblies.Clear();
            var root = new FakeNode();

            foreach (var pair in typesDictionary)
            {
                IEnumerable<TypeDefinition> types = pair.Value;

                var assemblyNode = new AssemblyNode(pair.Key);
                Rec(assemblyNode, "", types);

                root.Children.Add(assemblyNode);
                Assemblies.Add(assemblyNode);
            }

            root.IsIncluded = true;
 
        }

        public void Rec(RecursiveNode parent, 
            string currentNamespace, IEnumerable<TypeDefinition> types)
        {
            var groups = types.Where(t => t.Namespace != currentNamespace &&
                                          t.Namespace.StartsWith(currentNamespace))
                .OrderBy(t => t.Namespace)
                .GroupBy(t => ExtractNextNamespacePart(t.Namespace, currentNamespace));

            var leafTypes = types.Where(t => t.Namespace == currentNamespace)
               .OrderBy(t => t.Name);



            if (currentNamespace != "" && groups.Count() == 1 && !leafTypes.Any())
            {
                var singleGroup = groups.Single();
                parent.Name = ConcatNamespace(parent.Name, singleGroup.Key);
                Rec(parent, ConcatNamespace(currentNamespace, singleGroup.Key), singleGroup);
            }
            else
            {
                foreach (var group in groups)
                {
                    var node = new TypeNamespaceNode(parent, group.Key);
                    Rec(node, ConcatNamespace(currentNamespace, group.Key), group);
                    parent.Children.Add(node);
                }



                foreach (var typeDefinition in leafTypes)
                {
                    var typeNode = new TypeNode(parent, typeDefinition.Name, typeDefinition);
                    parent.Children.Add(typeNode);
                    Types.Add(typeNode);
                }
            }
       

        }



        public string InnerNamespace(string namespaceName)
        {
            return namespaceName.Remove(0, namespaceName.LastIndexOf(".") + 1);
        }

        public string ConcatNamespace(string one , string two)
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


        public string Remove(string namespaceName, string toRemove)
        {
            return namespaceName.Remove(0, namespaceName.LastIndexOf(".") + 1);
        }



        public IEnumerable<TypeDefinition> GetIncludedTypes()
        {
            return Types.Select(_ => _.TypeDefinition);


            //
            //            foreach (TypeNamespaceNode assembly in RootNamespaces)
            //            {
            //                AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(assembly.AssemblyFilePath);
            //                foreach (TypeNode type in assembly.Types.Where(t => t.IsLeafIncluded))
            //                {
            //                    yield return ad.MainModule.Types.Single(t => t.FullName == type.AssemblyFilePath);
            //                }
            //            }
        }
    }
}