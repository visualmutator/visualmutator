namespace VisualMutator.Model
{
    #region

    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Caching;
    using log4net;
    using Mutations;
    using Mutations.MutantsTree;
    using UsefulTools.Core;

    #endregion

    public interface IMutantsCache
    {
        void setDisabled( bool disableCache = false);

        ModulesProvider GetMutatedModules(Mutant mutant);
    }

    public class MutantsCache : IMutantsCache
    {
        private readonly IMutantsContainer _mutantsContainer;

        private readonly MemoryCache _cache;

 
        private const int MaxLoadedModules = 5;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _disableCache;

        //private IDictionary<Mutant, IList<IModule>> 


        public MutantsCache(IMutantsContainer mutantsContainer)
        {
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
                result = _mutantsContainer.ExecuteMutation(mutant, ProgressCounter.Inactive());
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