namespace VisualMutator.Model.StoringMutants
{
    #region

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using System.Windows.Documents;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Mutations;
    using Mutations.MutantsTree;
    using Ninject;
    using UsefulTools.Core;
    using Wintellect.PowerCollections;

    #endregion

    public interface IMutantsCache : IDisposable
    {
        void setDisabled( bool disableCache = false);


        Task<MutationResult> GetMutatedModulesAsync(Mutant mutant);
        void Release(MutationResult mutationResult);
    }

    public class MutantsCache : IMutantsCache
    {

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly OptionsModel _options;
        private readonly IWhiteSource _whiteCache;
        private readonly MutationSessionChoices _choices;
        private readonly IMutationExecutor _mutationExecutor;

        private readonly MemoryCache _cache;
       
        private const int MaxLoadedModules = 5;

        private bool _disableCache;
        private ConcurrentDictionary<string, ConcurrentBag<TaskCompletionSource<MutationResult>>> _map;


        [Inject]
        public MutantsCache(
            OptionsModel options,
            IWhiteSource whiteCache,
            MutationSessionChoices choices, 
            IMutationExecutor mutationExecutor)
        {
            _options = options;
            _whiteCache = whiteCache;
            _choices = choices;
            _mutationExecutor = mutationExecutor;

            _disableCache = !_options.MutantsCacheEnabled;
            var config = new NameValueCollection
                         {
                             {"physicalMemoryLimitPercentage", "40"},
                             {"cacheMemoryLimitMegabytes", "256"}
                         };

            _cache = new MemoryCache("CustomCache", config);
            _map = new ConcurrentDictionary<string, ConcurrentBag<TaskCompletionSource<MutationResult>>>();
        }

        public void setDisabled(bool disableCache = false)
        {
            _disableCache = disableCache;
        }
    
        //TODO:test error behaviour
        public async Task<MutationResult> GetMutatedModulesAsync(Mutant mutant)
        {
            try
            {
                _log.Debug("GetMutatedModules in object: " + ToString() + GetHashCode());
                _log.Info("Request to cache for mutant: " + mutant.Id);
                bool creating = false;
                MutationResult result;
                if (_disableCache || !_cache.Contains(mutant.Id))
                {
                    _log.Debug("Cache#Mutant " + mutant.Id + " not yet in cache.");
                    Task<MutationResult> resultTask;
                    ConcurrentBag<TaskCompletionSource<MutationResult>> awaitQueue;
                    if (_map.TryGetValue(mutant.Id, out awaitQueue))
                    {
                        _log.Debug("Cache#Mutant " + mutant.Id + " is being awaited for. Subscribing.");
                        var tcs = new TaskCompletionSource<MutationResult>();
                        awaitQueue.Add(tcs);
                        resultTask = tcs.Task;
                    }
                    else
                    {
                        _log.Debug("Cache#Mutant " + mutant.Id + " is not waited for. Creating subscriber.");
                        _map.TryAdd(mutant.Id, new ConcurrentBag<TaskCompletionSource<MutationResult>>());
                        resultTask = CreateNew(mutant);
                        creating = true;
                    }
                    _log.Debug("Cache#Mutant " + mutant.Id + ": Task");
                    result = await resultTask;

                    if (creating)
                    {
                        _log.Debug("Cache#Mutant " + mutant.Id + ": Creation complete. Trying to remove subscribers queue");
                        ConcurrentBag<TaskCompletionSource<MutationResult>> awaiters;
                        if(_map.TryRemove(mutant.Id, out awaiters))
                        {
                            _log.Debug("Cache#Mutant " + mutant.Id + string.Format(": Subscribers queue removed. Setting result on {0} subscribers", awaiters.Count));
                            foreach (var tcs in awaiters)
                            {
                                tcs.SetResult(result);
                            }
                        }
                        else
                        {
                            _log.Debug("Cache#Mutant " + mutant.Id + ": Could not remove subscribers queue");
                        }
                    }
                    return result;
                }
                else
                {
                    _log.Debug("Cache contains value for mutant " + mutant.Id);
                    result = (MutationResult) _cache.Get(mutant.Id);
                }
                return result;
            }
            catch (Exception e)
            {
                _log.Error(e);
                throw;
            }
           
        }

        public void Release(MutationResult mutationResult)
        {
            var cci = (CciModuleSource)mutationResult.MutatedModules;
                        _whiteCache.ReturnToCache(
                            cci.Modules.Single().Name,
                            cci);
            //  var tt = mutationResult.WhiteModules.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Range");
            //tt.ToString();
            //  var type = cci.Modules.Single().Module.GetAllTypes().Single(t => t.Name.Value == "Deque") as NamedTypeDefinition;
            //  var method = type.Methods.Single(m => m.Name.Value == "EnqueueFront");
            //
            //            var whiteCci = _choices.WhiteSource;
            //            var c = new CodeDeepCopier(whiteCci.Host);
            //            c.
            //            MethodDefinition methodDefinition = c.Copy(mutationResult.Mutant.MutationTarget.MethodRaw);
            //            var v = new Viss(mutationResult.MutatedModules.Host, methodDefinition);
            //            var modClean = v.Rewrite(mutationResult.MutatedModules.Modules.Single().Module);
            //
            //            var cci = (CciModuleSource) mutationResult.MutatedModules;
            //            cci.ReplaceWith(modClean);
            //            _whiteCache.ReturnToCache(
            //                cci.Modules.Single().Name,
            //                cci);
        }

        public class Viss : CodeRewriter
        {
            private readonly IMethodDefinition _sourceMethod;

            public Viss(IMetadataHost host, IMethodDefinition sourceMethod)
                : base(host, false)
            {
                _sourceMethod = sourceMethod;
            }

            public override IMethodDefinition Rewrite(IMethodDefinition method)
            {
                
                if (MemberHelper.GetMethodSignature(method) == MemberHelper.GetMethodSignature(_sourceMethod))
                {
                    return _sourceMethod;
                }
                return method;
            }
        }
        private async Task<MutationResult> CreateNew(Mutant mutant)
        {
            MutationResult result;
            if (mutant.MutationTarget == null || mutant.MutationTarget.ProcessingContext == null)
            {
                result = new MutationResult(mutant, new CciModuleSource(), null, null);
            }
            else
            {
            
                if(_options.ParsedParams.LegacyCreation)
                {
                    _log.Debug("Cache#Mutant " + mutant.Id + ": Awaiting white cache.");
                    var cci = await _whiteCache.GetWhiteModulesAsyncOld();

                    _log.Debug("Cache#Mutant " + mutant.Id + ": Awaiting mutation start.");
                    result = await _mutationExecutor.ExecuteMutation(mutant, cci);
                    _log.Debug("Cache#Mutant " + mutant.Id + ": Awaiting mutation finished.");
                }
                else
                {
                    _log.Debug("Cache#Mutant " + mutant.Id + ": Awaiting white cache.");
                    var cci = await _whiteCache.GetWhiteSourceAsync(mutant.MutationTarget.ProcessingContext.ModuleName);
                    _log.Debug("Cache#Mutant " + mutant.Id + ": taken source: "+cci.Guid);
                    _log.Debug("Cache#Mutant " + mutant.Id + ": Awaiting mutation start.");
                    result = await _mutationExecutor.ExecuteMutation(mutant, cci);
                    _log.Debug("Cache#Mutant " + mutant.Id + ": Awaiting mutation finished.");
                }



                if (!_disableCache)
                {
                    _cache.Add(new CacheItem(mutant.Id, result), new CacheItemPolicy());
                }
            }
            return result;
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}