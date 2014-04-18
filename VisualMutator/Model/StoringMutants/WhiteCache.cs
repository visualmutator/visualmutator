using System.Threading.Tasks;

namespace VisualMutator.Model.StoringMutants
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Exceptions;
    using Infrastructure;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.Ast;
    using Mutations.Types;

    public class WhiteCache : IDisposable, IWhiteCache
    {
        private readonly BlockingCollection<CciModuleSource> _whiteCache;
        private BlockingCollection<IList<string>> _paths;
        private IList<string> _assembliesPaths;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int _lowerBound;
        private int _currentCount;
        private IDisposable _timerRisposable;
        private Queue<TaskCompletionSource<CciModuleSource>> _clients; 
        public WhiteCache()
        {
            _whiteCache = new BlockingCollection<CciModuleSource>(20);
            _clients = new Queue<TaskCompletionSource<CciModuleSource>>();
            _lowerBound = 8;
        }

        public void Initialize(IList<string> assembliesPaths)
        {
            _assembliesPaths = assembliesPaths;
            _paths = new BlockingCollection<IList<string>>();
            _paths.Add(_assembliesPaths);
            
            //   _workers = new WorkerCollection<CciModuleSource>();

                new Thread(() =>
                {
                    foreach (IList<string> item in _paths.GetConsumingEnumerable())
                    {
                        Monitor.Enter(this);
                        while (_whiteCache.Count >= _lowerBound)
                        {
                            Monitor.Wait(this);
                        }
                        Monitor.Exit(this);
                        IList<string> item1 = item;
                        Task.Run(() => _whiteCache.TryAdd(CreateSource(item1)))
                            .ContinueWith(task =>
                        {
                            _paths.Add(item1);
                            NotifyClients();
                        }).LogErrors();
                            // _whiteCache.TryAdd(CreateSource(assembliesPaths));
                    }
                    
                }).Start();
           

//
//            _timerRisposable = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(200))
//                .Subscribe(ev =>
//                {
//                    if (_whiteCache.Count < _lowerBound)
//                    {
//                        // Task.Run(() => _whiteCache.TryAdd(CreateSource(assembliesPaths)));
//                        _whiteCache.TryAdd(CreateSource(assembliesPaths));
//                    }
//                     
//                });
        }

        private void NotifyClients()
        {
            lock (this)
            {
                if(_clients.Count > 0)
                {
                    CciModuleSource cciModuleSource = TryTake();
                    if(cciModuleSource != null)
                    {
                        _clients.Dequeue().TrySetResult(cciModuleSource);
                    }
                }
            }
        }

        public CciModuleSource GetWhiteModules()
        {
            CciModuleSource cciModuleSource = _whiteCache.Take();
            Monitor.Enter(this);
            Monitor.Pulse(this);
            Monitor.Exit(this);
            return cciModuleSource;

        }
        public Task<CciModuleSource> GetWhiteModulesAsync()
        {
            CciModuleSource cciModuleSource = TryTake();
            if (cciModuleSource != null)
            {
                return Task.FromResult(cciModuleSource);
            }
            else
            {
                var tcs = new TaskCompletionSource<CciModuleSource>();
                lock(this)
                {
                    _clients.Enqueue(tcs);
                }
                return tcs.Task;
            }
        }
        private CciModuleSource TryTake()
        {
            CciModuleSource cciModuleSource;
            if (_whiteCache.TryTake(out cciModuleSource))
            {
                Monitor.Enter(this);
                Monitor.Pulse(this);
                Monitor.Exit(this);
                return cciModuleSource;
            }
            return null;
        }
        public void Reinitialize(List<string> assembliesPaths)
        {
            //todo: remove other assemblies
        }

        public CciModuleSource CreateSource(IList<string> assembliesPaths)
        {
            var moduleSource = new CciModuleSource();
//            Parallel.ForEach(assembliesPaths, path =>
//            {
//                try
//                {
//                    moduleSource.AppendFromFile(path);
//
//                }
//                catch (AssemblyReadException e)
//                {
//                    _log.Warn("ReadAssembly failed. ", e);
//                }
//                catch (Exception e)
//                {
//                    _log.Warn("ReadAssembly failed. ", e);
//                }
//            });
            foreach (var assembliesPath in assembliesPaths)
            {
                try
                {
                    moduleSource.AppendFromFile(assembliesPath);

                }
                catch (AssemblyReadException e)
                {
                    _log.Warn("ReadAssembly failed. ", e);
                }
                catch (Exception e)
                {
                    _log.Warn("ReadAssembly failed. ", e);
                }

            }
            return moduleSource;
        }

        public void Dispose()
        {
            _paths.CompleteAdding();
            if (_timerRisposable != null)
            {
                _timerRisposable.Dispose();
            }
        }
    }
}