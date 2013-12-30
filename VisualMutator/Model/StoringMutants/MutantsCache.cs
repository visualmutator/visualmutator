namespace VisualMutator.Model
{
    #region

    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Caching;
    using log4net;
    using Mutations;
    using Mutations.MutantsTree;
    using StoringMutants;
    using UsefulTools.Core;

    #endregion

    public interface IMutantsCache
    {
        void setDisabled( bool disableCache = false);

        ModulesProvider GetMutatedModules(Mutant mutant);
        WhiteCache WhiteCache { get; }
    }

    public class MutantsCache : IMutantsCache
    {
        private readonly WhiteCache _whiteCache;
        private readonly IMutantsContainer _mutantsContainer;

        private readonly MemoryCache _cache;
       

 
        private const int MaxLoadedModules = 5;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _disableCache;

        //private IDictionary<Mutant, IList<IModule>> 
        public WhiteCache WhiteCache
        {
            get { return _whiteCache; }
        }
        public MutantsCache(IMutantsContainer mutantsContainer)
            : this(new WhiteCache(), mutantsContainer)
        {
            
        }
        public MutantsCache(WhiteCache whiteCache,IMutantsContainer mutantsContainer)
        {
            _whiteCache = whiteCache;
            _mutantsContainer = mutantsContainer;
            var config = new NameValueCollection();
 
            config.Add("physicalMemoryLimitPercentage", "50");
            config.Add("cacheMemoryLimitMegabytes", "256");

            _cache = new MemoryCache("CustomCache", config);
        }

        public void setDisabled(bool disableCache = false)
        {
            _disableCache = disableCache;
        }

        public ModulesProvider GetMutatedModules(Mutant mutant)
        {
            _log.Info("Request to cache for mutant: "+mutant.Id);
           // return _mutantsContainer.ExecuteMutation(mutant, _originalCode.Assemblies, _allowedTypes.ToList(), ProgressCounter.Inactive());
            
            ModulesProvider result;
            if (!_cache.Contains(mutant.Id) || _disableCache)
            {
                result = _mutantsContainer.ExecuteMutation(mutant, 
                    ProgressCounter.Inactive(), _whiteCache.GetWhiteModules());

                _cache.Add(new CacheItem(mutant.Id, result), new CacheItemPolicy());
            }
            else
            {
                result = (ModulesProvider)_cache.Get(mutant.Id);
            }
            return result;
        }

    }
}