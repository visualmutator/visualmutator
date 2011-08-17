namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

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
    }

    public class SolutionTypesManager : ITypesManager
    {
        public SolutionTypesManager()
        {
            Assemblies = new BetterObservableCollection<AssemblyNode>();
        }

        public BetterObservableCollection<AssemblyNode> Assemblies { get; set; }

        public void RefreshTypes(IEnumerable<string> projectsPaths)
        {
            Assemblies.Clear();
            foreach (string path in projectsPaths)
            {
                AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(path);

                var node = new AssemblyNode(ad.Name.Name, path);
                foreach (TypeDefinition typ in ad.MainModule.Types)
                {
                    node.Types.Add(new TypeNode(typ.FullName, typ.Name));
                }
                Assemblies.Add(node);
            }
        }

        public IEnumerable<TypeDefinition> GetIncludedTypes()
        {
            foreach (AssemblyNode assembly in Assemblies)
            {
                AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(assembly.FullPath);
                foreach (TypeNode type in assembly.Types.Where(t => t.IsIncluded))
                {
                    yield return ad.MainModule.Types.Single(t => t.FullName == type.FullName);
                }
            }
        }
    }
}