namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Text;

    using CommonUtilityInfrastructure;

    using Microsoft.Cci;

    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.MutantsTree;
    using log4net;

    public interface IMutantsCache
    {
        void Initialize(AssembliesProvider originalCode, ICollection<TypeIdentifier> allowedTypes, bool disableCache = false);

        AssembliesProvider GetMutatedModules(Mutant mutant);
    }

    public class MutantsCache : IMutantsCache
    {
        private readonly IMutantsContainer _mutantsContainer;

        private MemoryCache _cache;

        private AssembliesProvider _originalCode;

        private ICollection<TypeIdentifier> _allowedTypes;

        private const int MaxLoadedModules = 5;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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

        public void Initialize(AssembliesProvider originalCode, ICollection<TypeIdentifier> allowedTypes, bool disableCache = false)
        {
            _originalCode = originalCode;
            _allowedTypes = allowedTypes;
            _disableCache = disableCache;
        }

        public AssembliesProvider GetMutatedModules(Mutant mutant)
        {
            _log.Info("Request to cache for mutant: "+mutant.Id);
           // return _mutantsContainer.ExecuteMutation(mutant, _originalCode.Assemblies, _allowedTypes.ToList(), ProgressCounter.Inactive());
            
            AssembliesProvider result;
            if (!_cache.Contains(mutant.Id) || _disableCache)
            {
                result = _mutantsContainer.ExecuteMutation(mutant, _originalCode.Assemblies, _allowedTypes.ToList(), ProgressCounter.Inactive());
                _cache.Add(new CacheItem(mutant.Id, result), new CacheItemPolicy());
            }
            else
            {
                result = (AssembliesProvider)_cache.Get(mutant.Id);
            }
            return result;
            
        }


    }
}