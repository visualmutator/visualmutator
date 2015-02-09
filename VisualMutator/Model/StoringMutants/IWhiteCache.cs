namespace VisualMutator.Model.StoringMutants
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWhiteSource
    {
        Task Initialize();
        Task<List<CciModuleSource>> GetWhiteModulesAsync();
        void Dispose();
        void Pause(bool paused);
        Task<CciModuleSource> GetWhiteSourceAsync(string moduleName);
        void ReturnToCache(string name, CciModuleSource whiteModules);
        Task<List<CciModuleSource>> GetWhiteModulesAsyncOld();
    }
}