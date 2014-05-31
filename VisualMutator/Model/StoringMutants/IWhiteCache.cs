namespace VisualMutator.Model.StoringMutants
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWhiteCache
    {
        void Initialize();
        CciModuleSource GetWhiteModules();
        Task<CciModuleSource> GetWhiteModulesAsync();
        void Dispose();
        void Pause(bool paused);
    }
}