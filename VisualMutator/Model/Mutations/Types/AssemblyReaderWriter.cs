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