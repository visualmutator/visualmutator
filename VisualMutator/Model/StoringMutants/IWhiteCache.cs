namespace VisualMutator.Model.StoringMutants
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWhiteCache
    {
        Task Initialize();
        Task<CciModuleSource> GetWhiteModulesAsync();
        void Dispose();
        void Pause(bool paused);
        Task<CciModuleSource> GetWhiteModulesAsync(string moduleName);
    }
}