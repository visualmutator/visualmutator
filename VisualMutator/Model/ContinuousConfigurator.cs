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
        private readonly IRootFactory<ContinuousConfiguration> _continuousConfigurationFactory;
        private IObjectRoot<ContinuousConfiguration> _configuration;

        public ContinuousConfigurator(
            IOptionsManager optionsManager,
            IRootFactory<ContinuousConfiguration> continuousConfigurationFactory)
        {
            _optionsManager = optionsManager;
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
            _configuration = _continuousConfigurationFactory.CreateWithBindings(optionsModel);
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