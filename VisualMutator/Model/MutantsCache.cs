namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Text;

    using CommonUtilityInfrastructure;

    using Microsoft.Cci;

    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.MutantsTree;

    public interface IMutantsCache
    {
        void Initialize(AssembliesProvider originalCode, IList<TypeIdentifier> allowedTypes);

        AssembliesProvider GetMutatedModules(Mutant mutant);
    }

    public class MutantsCache : IMutantsCache
    {
        private readonly IMutantsContainer _mutantsContainer;

        private MemoryCache _cache;

        private AssembliesProvider _originalCode;

        private IList<TypeIdentifier> _allowedTypes;

        private const int MaxLoadedModules = 5;


        //private IDictionary<Mutant, IList<IModule>> 


        public MutantsCache(IMutantsContainer mutantsContainer)
        {
            _mutantsContainer = mutantsContainer;
            var config = new NameValueCollection();
         //   config.Add("pollingInterval", "00:05:00");
            config.Add("physicalMemoryLimitPercentage", "50");
            config.Add("cacheMemoryLimitMegabytes", "256");

   
            _cache = new MemoryCache("CustomCache", config);
        }

        public void Initialize(AssembliesProvider originalCode, IList<TypeIdentifier> allowedTypes)
        {
            _originalCode = originalCode;
            _allowedTypes = allowedTypes;
          //  _cache.CacheMemoryLimit = (long) 256.MiB().ToBytes();
        }

        public AssembliesProvider GetMutatedModules(Mutant mutant)
        {
            AssembliesProvider result;
            if(!_cache.Contains(mutant.Id))
            {
                result = _mutantsContainer.ExecuteMutation(mutant, _originalCode.Assemblies, _allowedTypes, ProgressCounter.Inactive());
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