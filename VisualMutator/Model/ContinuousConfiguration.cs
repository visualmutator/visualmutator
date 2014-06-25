namespace VisualMutator.Model
{
    using System;
    using Infrastructure;
    using Mutations.Operators;
    using StoringMutants;
    using UsefulTools.DependencyInjection;

    public class ContinuousConfiguration : IDisposable
    {
        private readonly IWhiteCache _whiteCache;
        private readonly IRootFactory<SessionConfiguration> _sessionConfigurationFactory;

        public ContinuousConfiguration(
            IWhiteCache whiteCache,
            OperatorsManager operatorsManager,
            IRootFactory<SessionConfiguration> sessionConfigurationFactory)
        {
            _whiteCache = whiteCache;
            _sessionConfigurationFactory = sessionConfigurationFactory;

            operatorsManager.GetOperators();
        }

        public IObjectRoot<SessionConfiguration> CreateSessionConfiguration()
        {
                return _sessionConfigurationFactory.Create();
        }

        public void Dispose()
        {
            _whiteCache.Dispose();
        }
    }
}