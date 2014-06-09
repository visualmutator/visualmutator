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

        private class WhiteClient
        {
            private TaskCompletionSource<CciModuleSource> _tcs;
            private string _key;

            public WhiteClient(string key)
            {
                _key = key;
                _tcs = new TaskCompletionSource<CciModuleSource>();
            }

            public string Key
            {
                get { return _key; }
            }

            public TaskCompletionSource<CciModuleSource> Tcs
            {
                get { return _tcs; }
            }
        }

        private class MiniCache
        {
            private BlockingCollection<CciModuleSource> _cache;
            private int _maxCount;




        }


        private readonly IProjectClonesManager _fileManager;
        private readonly OptionsModel _options;
       // private readonly BlockingCollection<CciModuleSource> _whiteCache;
        private BlockingCollection<ProjectFilesClone> _paths;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int _maxCount;
        private readonly Queue<WhiteClient> _clients;
        private List<ProjectFilesClone> _filesPool;
        private readonly int _threadsCount;
        private Exception _error;
        private readonly Dictionary<string, string> _moduleNameToFileName;
        private readonly ConcurrentDictionary<string, BlockingCollection<CciModuleSource>> _whiteCaches;
        private bool _initialized;
        private Task<CciModuleSource> _mainModules;
        private readonly int _initialMaxCount;

        public WhiteCache(
            IProjectClonesManager fileManager,
            OptionsModel options,
            // ------
            int threadsCount,
            int maxCount
            )
        {
            _fileManager = fileManager;
            _options = options;
            _threadsCount = threadsCount;
            _moduleNameToFileName = new Dictionary<string, string>();
            _whiteCaches = new ConcurrentDictionary<string, BlockingCollection<CciModuleSource>>();
            _clients = new Queue<WhiteClient>();
            _maxCount = maxCount;
            _initialMaxCount = maxCount;
            _paths = new BlockingCollection<ProjectFilesClone>();
        }

        public async Task Initialize()
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

            ProjectFilesClone filesClone = _paths.Take();
            _mainModules = Task.Run(() =>
            {
                var source = new CciModuleSource(filesClone);
                _paths.Add(filesClone);
                return source;
            });
//            try
//            {
//                await Task.Run(() => );
//            }
//            catch (Exception e)
//            {
//                _log.Error( e);
//                _error = e;
//                _paths.CompleteAdding();
//            }

            new Thread(() =>
            {
                try
                {
                    InitializeModuleNames();


                    foreach (ProjectFilesClone item in _paths.GetConsumingEnumerable())
                    {
                        Monitor.Enter(this);
                        while (_whiteCaches.All(_ => _.Value.Count >= _maxCount))
                        {
                            Monitor.Wait(this);
                        }
                        Monitor.Exit(this);
                        if(_paths.IsAddingCompleted)
                        {
                            return;
                        }
                        ProjectFilesClone item1 = item;
                        Task.Run(() => TryAdd(item1))
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

        private void InitializeModuleNames()
        {
            ProjectFilesClone projectFilesClone = _paths.Take();
            foreach (var path in projectFilesClone.Assemblies)
            {
                var cci = new CciModuleSource(path.Path);
                _moduleNameToFileName.Add(cci.Modules.Single().Name, path.FileName);
                var queue = new BlockingCollection<CciModuleSource>(20) {cci};
                _whiteCaches.TryAdd(path.FileName, queue);
            }
            _paths.Add(projectFilesClone);
            _initialized = true;
            NotifyClients();
            
        }

        private void TryAdd(ProjectFilesClone item1)
        {
            foreach (var whiteCach in _whiteCaches)
            {
                if(whiteCach.Value.Count < _maxCount)
                {
                    var filePath = item1.Assemblies.Single(_ => _.FileName == whiteCach.Key);
                    whiteCach.Value.Add(new CciModuleSource(filePath.Path));
                }
            }
        }

        private void NotifyClients()
        {
            lock (this)
            {
                if(_clients.Count > 0)
                {
                    CciModuleSource cciModuleSource = TryTake(_clients.Peek().Key);
                    if(cciModuleSource != null)
                    {
                        _clients.Dequeue().Tcs.TrySetResult(cciModuleSource);
                    }
                }
            }
        }
        public Task<CciModuleSource> GetWhiteModulesAsync()
        {
            return _mainModules; //GetWhiteModulesAsync(_whiteCaches.Single().Key);
        }

        private CciModuleSource TryTake(string moduleName)
        {
            if(!_initialized)
            {
                return null; //Not yet initialized
            }
            else
            {
                string fileName = _moduleNameToFileName[moduleName];
                CciModuleSource cciModuleSource;
                if (_whiteCaches.ContainsKey(fileName) &&
                    _whiteCaches[fileName].TryTake(out cciModuleSource, TimeSpan.FromMilliseconds(10)))
                {
                    Pulse();
                    return cciModuleSource;
                }
                return null;
            }
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
                    _maxCount = _initialMaxCount;
                    Pulse();
                }
                else
                {
                    _maxCount = 1;
                }
            }
        }

        public Task<CciModuleSource> GetWhiteModulesAsync(string moduleName)
        {
            
            CciModuleSource cciModuleSource = TryTake(moduleName);
            if (cciModuleSource != null)
            {
                return Task.FromResult(cciModuleSource);
            }
            else
            {
                var client = new WhiteClient(moduleName);
                if (_error != null)
                {
                    client.Tcs.SetException(_error);
                }
                else
                {
                    lock (this)
                    {
                        _clients.Enqueue(client);
                    }
                }
                return client.Tcs.Task;
            }
        }

       

        public void Dispose()
        {
            _log.Debug("Disposing white cache");
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