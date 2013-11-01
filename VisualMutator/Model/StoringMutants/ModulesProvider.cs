namespace VisualMutator.Model
{
    #region

    using System.Collections.Generic;
    using Microsoft.Cci;

    #endregion

    public class ModulesProvider
    {
        public ModulesProvider(IList<IModule> modules)
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