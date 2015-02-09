namespace VisualMutator.Tests.Operators
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Model;
    using Model.StoringMutants;

    public class WhiteDummy : IWhiteSource
    {
        private readonly List<string> _paths;

        public WhiteDummy(List<string> paths)
        {
            _paths = paths;
        }

        public Task Initialize()
        {
            return Task.FromResult(0);
        }

        public Task<List<CciModuleSource>> GetWhiteModulesAsync()
        {
            return Task.FromResult(_paths.Select(p => new CciModuleSource(p)).ToList());

        }

        public void Dispose()
        {
        }

        public void Pause(bool paused)
        {
        }

        public Task<CciModuleSource> GetWhiteSourceAsync(string moduleName)
        {
            var path = _paths.Single(p => Path.GetFileNameWithoutExtension(p) == moduleName);
            return Task.FromResult(new CciModuleSource(path));
        }

        public void ReturnToCache(string name, CciModuleSource whiteModules)
        {
        }

        public Task<List<CciModuleSource>> GetWhiteModulesAsyncOld()
        {
            return null;
        }
    }
}