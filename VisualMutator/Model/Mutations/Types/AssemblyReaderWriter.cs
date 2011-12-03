namespace VisualMutator.Model.Mutations.Types
{
    using System;
    using System.IO;

    using Mono.Cecil;

    using VisualMutator.Model.Exceptions;

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
            try
            {
                return AssemblyDefinition.ReadAssembly(path);
            }
            catch (FileNotFoundException e)
            {
                throw new AssemblyReadException("",e);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new AssemblyReadException("", e);
            }
            catch (IOException e)
            {
                throw new AssemblyReadException("", e);
            }
            
        }
        public void WriteAssembly(AssemblyDefinition assembly, string path)
        {
            assembly.Write(path);
        }
    }
}