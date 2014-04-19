namespace VisualMutator.Model
{
    using System;
    using Infrastructure;
    using StoringMutants;
    using UsefulTools.DependencyInjection;

    public class ContinuousConfiguration : IDisposable
    {
        private readonly IBindingFactory<SessionConfiguration> _sessionConfigurationFactory;
        private readonly IWhiteCache _whiteCache;

        public ContinuousConfiguration(
            IOptionsManager optionsManager,
            IBindingFactory<SessionConfiguration> sessionConfigurationFactory,
            IFactory<WhiteCache> whiteCacheFactory)
        {
            _sessionConfigurationFactory = sessionConfigurationFactory;
            OptionsModel options = optionsManager.ReadOptions();
            if (options.WhiteCacheThreadsCount != 0)
            {
                _whiteCache = whiteCacheFactory.CreateWithParams(options.WhiteCacheThreadsCount);
            }
            else
            {
                _whiteCache = new DisabledWhiteCache();
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