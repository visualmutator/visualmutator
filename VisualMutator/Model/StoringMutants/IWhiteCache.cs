namespace VisualMutator.Model.StoringMutants
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWhiteCache
    {
        void Initialize();
        CciModuleSource GetWhiteModules();
        void Reinitialize(List<string> assembliesPaths);
        Task<CciModuleSource> GetWhiteModulesAsync();
        void Dispose();
        bool Paused {  set; }
    }
}