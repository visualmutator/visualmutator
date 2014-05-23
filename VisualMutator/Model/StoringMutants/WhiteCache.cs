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
        private BlockingCollection<IList<string>> _paths;
        private ProjectFilesClone _assembliesPaths;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly int _maxCount;
        private readonly Queue<TaskCompletionSource<CciModuleSource>> _clients;
        private List<ProjectFilesClone> _filesPool;
        private readonly int _threadsCount;
        private bool _paused;

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
            _maxCount = 8;
            _paths = new BlockingCollection<IList<string>>();
        }

        public async void Initialize()
        {
            _assembliesPaths = _fileManager.CreateClone("WhiteCache-" );
            _paths = new BlockingCollection<IList<string>>();
            _paths.Add(_assembliesPaths.Assemblies.Select(_ => _.Path).ToList());

            ProjectFilesClone[] projectFilesClones = await Task.WhenAll(
                Enumerable.Range(0, _threadsCount)
                .Select(i => _fileManager.CreateCloneAsync("WhiteCache-"+i)));
            
            _filesPool = projectFilesClones.ToList();
            foreach (var projectFilesClone in _filesPool)
            {
                _paths.Add(projectFilesClone.Assemblies.Select(_ => _.Path).ToList());
            }
                       

            new Thread(() =>
            {
                foreach (IList<string> item in _paths.GetConsumingEnumerable())
                {
                    Monitor.Enter(this);
                    while (_whiteCache.Count >= _maxCount || _paused)
                    {
                        Monitor.Wait(this);
                    }
                    Monitor.Exit(this);
                    IList<string> item1 = item;
                    Task.Run(() => _whiteCache.TryAdd(CreateSource(item1)))
                        .ContinueWith(task =>
                    {
                        _paths.TryAdd(item1);
                        NotifyClients();
                    }).LogErrors();
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
            _assembliesPaths.Dispose();
            foreach (var projectFilesClone in _filesPool)
            {
                projectFilesClone.Dispose();
            }

        }

        public bool Paused
        {
            get { return _paused; }
            set { _paused = value; }
        }
    }
}