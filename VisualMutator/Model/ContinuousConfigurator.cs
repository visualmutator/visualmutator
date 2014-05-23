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

        public ContinuousConfigurator(
            IOptionsManager optionsManager,
            IRootFactory<ContinuousConfiguration> continuousConfigurationFactory)
        {
            _optionsManager = optionsManager;
            _continuousConfigurationFactory = continuousConfigurationFactory;
        }

        public IObjectRoot<ContinuousConfiguration> GetConfiguration()
        {
            var optionsModel = _optionsManager.ReadOptions();
            return _continuousConfigurationFactory.CreateWithBindings(optionsModel);
        }



    }
}