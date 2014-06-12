namespace VisualMutator.Model
{
    using System;
    using Infrastructure;
    using Mutations.Operators;
    using StoringMutants;
    using UsefulTools.DependencyInjection;

    public class ContinuousConfiguration : IDisposable
    {
        private readonly IRootFactory<SessionConfiguration> _sessionConfigurationFactory;
        private readonly IWhiteCache _whiteCache;

        public ContinuousConfiguration(
            OperatorsManager operatorsManager,
            IRootFactory<SessionConfiguration> sessionConfigurationFactory,
            IWhiteCache whiteCache)
        {
            _sessionConfigurationFactory = sessionConfigurationFactory;
            _whiteCache = whiteCache;

            operatorsManager.GetOperators();
        }

        public IObjectRoot<SessionConfiguration> CreateSessionConfiguration()
        {
            return _sessionConfigurationFactory.Create();
        }

        public void Dispose()
        {
        }
    }
}