namespace VisualMutator.Model
{
    using System.Collections.Generic;
    using Microsoft.Cci;
    using Mono.Cecil;

    public class AssembliesProvider
    {
        public List<IModule> Assemblies { get; set; }
    }
}