namespace VisualMutator.Model.StoringMutants
{
    using System.Collections.Generic;

    public interface IWhiteCache
    {
        void Initialize(IList<string> assembliesPaths);
        ModuleSource GetWhiteModules();
    }
}