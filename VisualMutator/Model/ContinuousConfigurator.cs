namespace VisualMutator.Model
{
    using System.Linq;
    using Infrastructure;
    using StoringMutants;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;

    public class ContinuousConfigurator
    {
        private readonly IOptionsManager _optionsManager;
        private readonly IFactory<WhiteCache> _whiteCacheFactory;
        private readonly IRootFactory<ContinuousConfiguration> _continuousConfigurationFactory;
        private IObjectRoot<ContinuousConfiguration> _configuration;

        public ContinuousConfigurator(
            IOptionsManager optionsManager,
            IFactory<WhiteCache> whiteCacheFactory,
            IRootFactory<ContinuousConfiguration> continuousConfigurationFactory)
        {
            _optionsManager = optionsManager;
            _whiteCacheFactory = whiteCacheFactory;
            _continuousConfigurationFactory = continuousConfigurationFactory;
        }



        public IObjectRoot<ContinuousConfiguration> GetConfiguration()
        {
            return _configuration;
        }


        public void CreateConfiguration()
        {
            DisposeConfiguration();
            var optionsModel = _optionsManager.ReadOptions();


            IWhiteSource whiteCache;
            if (optionsModel.WhiteCacheThreadsCount != 0)
            {
                whiteCache = _whiteCacheFactory.CreateWithParams(optionsModel.WhiteCacheThreadsCount, optionsModel.WhiteCacheThreadsCount);
            }
            else
            {
                whiteCache = _whiteCacheFactory.CreateWithParams(1, 1);
            }
            whiteCache.Initialize();
            _configuration = _continuousConfigurationFactory.CreateWithBindings(optionsModel, whiteCache);
        }

        public void DisposeConfiguration()
        {
            if (_configuration != null)
            {
                _configuration.Get.Dispose();
                _configuration = null;
            }
        }
    }
}