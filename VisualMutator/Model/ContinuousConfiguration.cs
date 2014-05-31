namespace VisualMutator.Model
{
    using System;
    using Infrastructure;
    using Mutations.Operators;
    using StoringMutants;
    using UsefulTools.DependencyInjection;

    public class ContinuousConfiguration : IDisposable
    {
        private readonly OperatorsManager _operatorsManager;
        private readonly IRootFactory<SessionConfiguration> _sessionConfigurationFactory;
        private readonly OptionsModel _optionsModel;
        private readonly IWhiteCache _whiteCache;

        public ContinuousConfiguration(
            OperatorsManager operatorsManager,
            IProjectClonesManager fileManager,
            IRootFactory<SessionConfiguration> sessionConfigurationFactory,
            IFactory<WhiteCache> whiteCacheFactory,
            IFactory<DisabledWhiteCache> disabledCacheFactory,
            //------
            OptionsModel optionsModel)
        {
            _operatorsManager = operatorsManager;
            _sessionConfigurationFactory = sessionConfigurationFactory;
            _optionsModel = optionsModel;

            if (_optionsModel.WhiteCacheThreadsCount != 0)
            {
                _whiteCache = whiteCacheFactory.CreateWithParams(_optionsModel.WhiteCacheThreadsCount);
            }
            else
            {
                _whiteCache = disabledCacheFactory.Create();
            }
            fileManager.Initialize();
            _whiteCache.Initialize();
            _operatorsManager.GetOperators();
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