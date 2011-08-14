namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

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