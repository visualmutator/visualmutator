namespace VisualMutator.Model
{
    using System.Linq;
    using Infrastructure;
    using StoringMutants;
    using UsefulTools.ExtensionMethods;

    public class ContinuousConfigurator
    {
        private readonly IOptionsManager _optionsManager;
        private readonly IBindingFactory<SessionConfiguration> _sessionConfigurationFactory;

        public ContinuousConfigurator(
            IOptionsManager optionsManager,
            IBindingFactory<SessionConfiguration> sessionConfigurationFactory)
        {
            _optionsManager = optionsManager;
            _sessionConfigurationFactory = sessionConfigurationFactory;
        }

        public SessionConfiguration CreateConfiguration()
        {
            OptionsModel options = _optionsManager.ReadOptions();
            IWhiteCache whiteCache;
            if (options.WhiteCacheEnabled)
            {
                whiteCache = new WhiteCache();
            }
            else
            {
                whiteCache = new DisabledWhiteCache();
            }

            return _sessionConfigurationFactory.CreateWithBindings(whiteCache);
        }


    }
}