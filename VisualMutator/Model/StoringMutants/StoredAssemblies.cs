namespace VisualMutator.Model.StoringMutants
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    public class StoredAssemblies
    {
        private readonly List<IModule> _modules;

        public List<IModule> Modules
        {
            get { return _modules; }
        }

        public StoredAssemblies(List<IModule> modules)
        {
            _modules = modules;
        }
    }
}