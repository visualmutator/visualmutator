namespace VisualMutator.Model.StoringMutants
{
    #region

    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Caching;
    using log4net;
    using Mutations;
    using Mutations.MutantsTree;
    using Ninject;
    using UsefulTools.Core;

    #endregion

    public interface IMutantsCache
    {
        void setDisabled( bool disableCache = false);

        IModuleSource GetMutatedModules(Mutant mutant);
        IWhiteCache WhiteCache
        {
            get;
        }
    }

    public class MutantsCache : IMutantsCache
    {
        private readonly MutationSessionChoices _choices;
        private readonly IWhiteCache _whiteCache;
        private readonly IMutantsContainer _mutantsContainer;

        private readonly MemoryCache _cache;
       
        private const int MaxLoadedModules = 5;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _disableCache;

        //private IDictionary<Mutant, IList<IModule>> 
        public IWhiteCache WhiteCache
        {
            get { return _whiteCache; }
        }

        public MutantsCache(
            
            IMutantsContainer mutantsContainer
            )
            : this(new MutationSessionChoices(), new DisabledWhiteCache(), mutantsContainer)
        {
        }

        [Inject]
        public MutantsCache(
            MutationSessionChoices choices, 
            IWhiteCache whiteCache,
            IMutantsContainer mutantsContainer)
        {
            _choices = choices;
            _whiteCache = whiteCache;
            _mutantsContainer = mutantsContainer;

            _disableCache = !_choices.MainOptions.MutantsCacheEnabled;
            var config = new NameValueCollection();
 
            config.Add("physicalMemoryLimitPercentage", "50");
            config.Add("cacheMemoryLimitMegabytes", "256");

            _cache = new MemoryCache("CustomCache", config);
        }

        public void setDisabled(bool disableCache = false)
        {
            _disableCache = disableCache;
        }

        public IModuleSource GetMutatedModules(Mutant mutant)
        {
            _log.Debug("GetMutatedModules in object: " + ToString() + GetHashCode());
            _log.Info("Request to cache for mutant: "+mutant.Id);
           // return _mutantsContainer.ExecuteMutation(mutant, _originalCode.Assemblies, _allowedTypes.ToList(), ProgressCounter.Inactive());
            
            IModuleSource result;
            if (_disableCache || !_cache.Contains(mutant.Id))
            {
                result = _mutantsContainer.ExecuteMutation(mutant, 
                    ProgressCounter.Inactive(), _whiteCache.GetWhiteModules());

                _cache.Add(new CacheItem(mutant.Id, result), new CacheItemPolicy());
            }
            else
            {
                result = (IModuleSource)_cache.Get(mutant.Id);
            }
            return result;
        }

    }
}