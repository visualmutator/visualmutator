namespace VisualMutator.Model.StoringMutants
{
    #region

    using System.Collections.Generic;
    using Microsoft.Cci;

    #endregion

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