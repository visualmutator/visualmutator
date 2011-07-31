namespace VisualMutator.Domain
{
    #region Usings

    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    #endregion

    public interface ITypesManager
    {
        ObservableCollection<AssemblyNode> Assemblies { get; set; }

        void RefreshTypes(IEnumerable<string> projectsPaths);
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
                var node = new AssemblyNode(ad);
                foreach (TypeDefinition typ in ad.MainModule.Types)
                {
                    node.Types.Add(new TypeNode(typ));


                }
                Assemblies.Add(node);
            }
        }






    }
}