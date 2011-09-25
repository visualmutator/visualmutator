namespace VisualMutator.Model.Mutations.Types
{
    using Mono.Cecil;

    public interface IAssemblyReaderWriter
    {
      //  IGrouping<string, TypeDefinition> GetTypesFromAssembly(string path, out AssemblyDefinition ad);
        AssemblyDefinition ReadAssembly(string path);

        void WriteAssembly(AssemblyDefinition assembly, string path);
    }

    public class AssemblyReaderWriter : IAssemblyReaderWriter
    {
        public AssemblyReaderWriter()
        {
            
        }

    /*
        public IEnumerable<IGrouping<string, TypeDefinition>> GetTypesFromAssembly(IEnumerable<string> paths,
             IEnumerable<AssemblyDefinition> assemblies)
        {

            var assemblies2 = paths.Select(path=> AssemblyDefinition.ReadAssembly(path));
            assemblies = assemblies2;
            foreach (var assemblyDefinition in assemblies2)
            {
                yield return assemblyDefinition.MainModule.Types
                .Where(t => t.Name != "<Module>").GroupBy(t=>t.Module.Assembly.Name.Name).Single();
            }
         

        }
        */
        public AssemblyDefinition ReadAssembly(string path)
        {
            return AssemblyDefinition.ReadAssembly(path);
        }
        public void WriteAssembly(AssemblyDefinition assembly, string path)
        {
            assembly.Write(path);
        }
    }
}