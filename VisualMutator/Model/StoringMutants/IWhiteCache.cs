namespace VisualMutator.Model.StoringMutants
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWhiteCache
    {
        Task Initialize();
        Task<List<CciModuleSource>> GetWhiteModulesAsync();
        void Dispose();
        void Pause(bool paused);
        Task<CciModuleSource> GetWhiteModulesAsync(string moduleName);
        void ReturnToCache(string name, CciModuleSource whiteModules);
    }
}