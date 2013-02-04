namespace VisualMutator.Model
{
    using System.Collections.Generic;
    using Mono.Cecil;

    public class AssembliesProvider
    {
        public List<AssemblyDefinition> Assemblies { get; set; }
    }
}