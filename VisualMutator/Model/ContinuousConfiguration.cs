namespace VisualMutator.Model
{
    using System;
    using Infrastructure;
    using StoringMutants;
    using UsefulTools.DependencyInjection;

    public class ContinuousConfiguration : IDisposable
    {
        private readonly IFileSystemManager _fileManager;
        private readonly IRootFactory<SessionConfiguration> _sessionConfigurationFactory;
        private readonly IWhiteCache _whiteCache;

        public ContinuousConfiguration(
             IFileSystemManager fileManager,
            IOptionsManager optionsManager,
            IRootFactory<SessionConfiguration> sessionConfigurationFactory,
            IFactory<WhiteCache> whiteCacheFactory,
            IFactory<DisabledWhiteCache> disabledCacheFactory)
        {
            _fileManager = fileManager;
            _sessionConfigurationFactory = sessionConfigurationFactory;

            OptionsModel options = optionsManager.ReadOptions();
            if (options.WhiteCacheThreadsCount != 0)
            {
                _whiteCache = whiteCacheFactory.CreateWithParams(options.WhiteCacheThreadsCount);
            }
            else
            {
                _whiteCache = disabledCacheFactory.Create();
            }
        }

        public SessionConfiguration CreateSessionConfiguration()
        {
            return _sessionConfigurationFactory.CreateWithBindings(_whiteCache);
        }

        public void Dispose()
        {
            _whiteCache.Dispose();
        }
    }
}