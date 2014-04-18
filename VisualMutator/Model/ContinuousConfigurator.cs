namespace VisualMutator.Model
{
    using System.Linq;
    using Infrastructure;
    using StoringMutants;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;

    public class ContinuousConfigurator
    {
        private readonly IBindingFactory<ContinuousConfiguration> _continuousConfigurationFactory;

        public ContinuousConfigurator(
            IBindingFactory<ContinuousConfiguration> continuousConfigurationFactory)
        {
            _continuousConfigurationFactory = continuousConfigurationFactory;
        }

        public ContinuousConfiguration GetConfiguration()
        {
           
            return _continuousConfigurationFactory.Create();
        }



    }
}