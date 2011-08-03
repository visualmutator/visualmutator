namespace VisualMutator.Domain
{
    #region Usings

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    #endregion

    public interface ITypesManager
    {
        ObservableCollection<AssemblyNode> Assemblies { get; set; }

        void RefreshTypes(IEnumerable<string> projectsPaths);

        IEnumerable<TypeDefinition> GetIncludedTypes();
    }

    public class SolutionTypesManager : ITypesManager
    {


        public ObservableCollection<AssemblyNode> Assemblies
        {
            get;
            set;
        }

        public SolutionTypesManager()
        {

            Assemblies =new ObservableCollection<AssemblyNode>();

        }

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
            foreach (var assembly in Assemblies)
            {
                var ad = AssemblyDefinition.ReadAssembly(assembly.FullPath);
                foreach (var type in assembly.Types.Where(t=> t.Included))
                {
                    yield return ad.MainModule.Types.Single(t => t.FullName == type.FullName);
                }

            }

        }


    }
}