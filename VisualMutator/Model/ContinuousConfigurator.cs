namespace VisualMutator.Model
{
    using System.Linq;
    using Infrastructure;
    using StoringMutants;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;

    public class ContinuousConfigurator
    {
        private readonly IRootFactory<ContinuousConfiguration> _continuousConfigurationFactory;

        public ContinuousConfigurator(
            IRootFactory<ContinuousConfiguration> continuousConfigurationFactory)
        {
            _continuousConfigurationFactory = continuousConfigurationFactory;
        }

        public IObjectRoot<ContinuousConfiguration> GetConfiguration()
        {
            return _continuousConfigurationFactory.Create();
        }



    }
}