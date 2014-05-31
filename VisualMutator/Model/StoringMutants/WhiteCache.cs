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
        private readonly IProjectClonesManager _fileManager;
        private readonly BlockingCollection<CciModuleSource> _whiteCache;
        private BlockingCollection<ProjectFilesClone> _paths;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int _maxCount;
        private readonly Queue<TaskCompletionSource<CciModuleSource>> _clients;
        private List<ProjectFilesClone> _filesPool;
        private readonly int _threadsCount;
        private Exception _error;

        public WhiteCache(
            IProjectClonesManager fileManager,
            // ------
            int threadsCount
            )
        {
            _fileManager = fileManager;
            _threadsCount = threadsCount;
            _whiteCache = new BlockingCollection<CciModuleSource>(20);
            _clients = new Queue<TaskCompletionSource<CciModuleSource>>();
            _maxCount = 5;
            _paths = new BlockingCollection<ProjectFilesClone>();
        }

        public async void Initialize()
        {
            _error = null;
            _paths = new BlockingCollection<ProjectFilesClone>();
            ProjectFilesClone[] projectFilesClones = await Task.WhenAll(
                Enumerable.Range(0, _threadsCount)
                .Select(i => _fileManager.CreateCloneAsync("WhiteCache-"+i)));
            
            _filesPool = projectFilesClones.ToList();
            foreach (var projectFilesClone in _filesPool)
            {
                _paths.Add(projectFilesClone);
            }
            
            new Thread(() =>
            {
                try
                {
                    foreach (ProjectFilesClone item in _paths.GetConsumingEnumerable())
                    {
                        Monitor.Enter(this);
                        while (_whiteCache.Count >= _maxCount)
                        {
                            Monitor.Wait(this);
                        }
                        Monitor.Exit(this);
                        if(_paths.IsAddingCompleted)
                        {
                            return;
                        }
                        ProjectFilesClone item1 = item;
                        Task.Run(() => _whiteCache.TryAdd(CreateSource(item1)))
                            .ContinueWith(task =>
                            {
                                _paths.TryAdd(item1);
                                NotifyClients();
                            }).LogErrors();
                    }
                }
                catch (Exception e)
                {
                    _log.Error("Read assembly failed. ", e);
                    _error = e;
                    _paths.CompleteAdding();
                }

            }).Start();
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
            Pulse();
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
                if (_error != null)
                {
                    tcs.SetException(_error);
                }
                else
                {
                    lock (this)
                    {
                        _clients.Enqueue(tcs);
                    }
                }
                return tcs.Task;
            }
        }

        private CciModuleSource TryTake()
        {
            CciModuleSource cciModuleSource;
            if (_whiteCache.TryTake(out cciModuleSource, TimeSpan.FromMilliseconds(10)))
            {
                Pulse();
                return cciModuleSource;
            }
            return null;
        }


        public CciModuleSource CreateSource(ProjectFilesClone assembliesPaths)
        {
            return new CciModuleSource(assembliesPaths);
        }
        public void Pause(bool pause)
        {
            lock (this)
            {
                if (!pause)
                {
                    _maxCount = 5;
                    Pulse();
                }
                else
                {
                    _maxCount = 1;
                }
            }
        }
        public void Dispose()
        {
            _paths.CompleteAdding();
            foreach (var projectFilesClone in _filesPool)
            {
                projectFilesClone.Dispose();
            }
            Pulse();
        }
        private void Pulse()
        {
            Monitor.Enter(this);
            Monitor.Pulse(this);
            Monitor.Exit(this);
        }
    }
}