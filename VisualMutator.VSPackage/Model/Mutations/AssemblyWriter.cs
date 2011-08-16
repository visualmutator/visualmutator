namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using Mono.Cecil;

    #endregion

    public interface IAssemblyWriter
    {
        void Write(string sessionName, AssemblyDefinition assembly);
    }

    public class AssemblyWriter : IAssemblyWriter
    {
        public AssemblyWriter(string rootFolder)
        {
            //_rootFolder = rootFolder;
        }

        public void Write(string sessionName, AssemblyDefinition assembly)
        {
        }
    }
}