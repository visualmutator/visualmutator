namespace VisualMutator.Model.StoringMutants
{
    #region

    using System.Collections.Generic;
    using Microsoft.Cci;

    #endregion

    public interface IModuleSource 
    {
        List<IModule> Modules
        {
            get;
        }
    }

    class SimpleModuleSource : IModuleSource
    {
        public SimpleModuleSource(List<IModule> modules)
        {
            Modules = modules;
        }

        public List<IModule> Modules { get; private set; }
    }
}