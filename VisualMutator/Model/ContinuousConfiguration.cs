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
        private readonly OptionsModel _optionsModel;
        private readonly IWhiteCache _whiteCache;

        public ContinuousConfiguration(
            OperatorsManager operatorsManager,
            IRootFactory<SessionConfiguration> sessionConfigurationFactory,
            IFactory<WhiteCache> whiteCacheFactory,
            IFactory<DisabledWhiteCache> disabledCacheFactory,
            //------
            OptionsModel optionsModel)
        {
            _sessionConfigurationFactory = sessionConfigurationFactory;
            _optionsModel = optionsModel;

            if (_optionsModel.WhiteCacheThreadsCount != 0)
            {
                _whiteCache = whiteCacheFactory.CreateWithParams(_optionsModel.WhiteCacheThreadsCount, 3);
            }
            else
            {
                _whiteCache = whiteCacheFactory.CreateWithParams(1, 1);
            }
            _whiteCache.Initialize();
            operatorsManager.GetOperators();
        }

        public IObjectRoot<SessionConfiguration> CreateSessionConfiguration()
        {
            return _sessionConfigurationFactory.CreateWithBindings(_whiteCache, _optionsModel);
        }

        public void Dispose()
        {
            _whiteCache.Dispose();
        }
    }
}