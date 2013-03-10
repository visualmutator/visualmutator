namespace VisualMutator.Model
{
    using System.Collections.Generic;
    using Microsoft.Cci;
    using Mono.Cecil;

    public class AssembliesProvider
    {
        public AssembliesProvider(IList<IModule> modules)
        {
            Assemblies = modules;
        }

        public IList<IModule> Assemblies
        {
            get;
            set;
        }
    }
}