using System.Threading.Tasks;

namespace VisualMutator.Model.StoringMutants
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using Exceptions;
    using Extensibility;
    using Infrastructure;
    using log4net;
    using stdole;

    public class WhiteCache : IDisposable, IWhiteSource
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


        private readonly IProjectClonesManager _fileManager;
        private readonly OptionsModel _options;
       // private readonly BlockingCollection<CciModuleSource> _whiteCache;
        private BlockingCollection<ProjectFilesClone> _paths;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int _maxCount;
        private readonly ConcurrentQueue<WhiteClient> _clients;
        private List<ProjectFilesClone> _filesPool;
        private readonly int _threadsCount;
        private Exception _error;
        private readonly Dictionary<string, string> _moduleNameToFileName;
        private readonly ConcurrentDictionary<string, BlockingCollection<CciModuleSource>> _whiteCaches;
        private bool _initialized;
        private Task<List<CciModuleSource>> _mainModules;
        private readonly int _initialMaxCount;
        public List<string> _referenceStrings;
        private Dictionary<string, string> _fileNameToModuleName;

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
            _fileNameToModuleName = new Dictionary<string, string>();
            _whiteCaches = new ConcurrentDictionary<string, BlockingCollection<CciModuleSource>>();
            _clients = new ConcurrentQueue<WhiteClient>();
            _maxCount = maxCount;
            _initialMaxCount = maxCount;
            _log.Debug("Whitecache Initializing with max count: "+ _initialMaxCount);
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
                    _referenceStrings = new List<string>();
                    var task = filesClone.Assemblies.Select(a =>
                    {
                        var cci = new CciModuleSource(a.Path);
                        cci.Guid = Guid.NewGuid();
                        _log.Debug("Whitecache#" + a.FileName + ": Created initial source: "+cci.Guid);
                        return cci;
                    }).ToList();

                    _paths.Add(filesClone);
                    return task;
                });
            

            new Thread(() =>
            {
                try
                {
                    InitializeModuleNames();


                    foreach (ProjectFilesClone item in _paths.GetConsumingEnumerable())
                    {
                        lock(this)
                        {
                            while (_whiteCaches.All(_ => _.Value.Count >= _maxCount) && !_paths.IsAddingCompleted)
                            {
                                Monitor.Wait(this);
                            }
                        }
                       
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
                _fileNameToModuleName.Add(path.FileName, cci.Modules.Single().Name);
                var queue = new BlockingCollection<CciModuleSource>(20) {cci};
                _whiteCaches.TryAdd(path.FileName, queue);
                _log.Debug("Whitecache#" + PrintCacheState(path.FileName) + "Initializing module name.");
            }
            _paths.Add(projectFilesClone);
            _initialized = true;
            NotifyClients();
            
        }

        private string PrintCacheState(string key)
        {
            if (_whiteCaches.ContainsKey(key))
            {
                return string.Format("[{0}]({1}/{2}): ", key, _whiteCaches[key].Count, _maxCount);
            }
            else
            {
                return "notfound: " + key+": ";
            }
        }

        private void TryAdd(ProjectFilesClone item1)
        {
            foreach (var whiteCach in _whiteCaches)
            {
                if (whiteCach.Value.Count < _maxCount)
                {
                    var guid = Guid.NewGuid();
                    _log.Debug("Whitecache#"+ PrintCacheState(whiteCach.Key) + "Add started.");
                    var filePath = item1.Assemblies.Single(_ => _.FileName == whiteCach.Key);
                    var cci = new CciModuleSource(filePath.Path);
                    cci.Guid = guid;
                    
                    whiteCach.Value.Add(cci);
                    _log.Debug("Whitecache#" + PrintCacheState(whiteCach.Key) + "Add finished: " + guid);
                }
            }
        }

        private void NotifyClients()
        {
            WhiteClient client;
            if(_clients.TryDequeue(out client))
            {
                CciModuleSource cciModuleSource = TryTake(client.Key);
                if (cciModuleSource != null)
                {
                    _log.Debug("Whitecache#" + PrintCacheState(_moduleNameToFileName[client.Key]) + "Source taken later: " + cciModuleSource.Guid);
                    client.Tcs.TrySetResult(cciModuleSource);
                }
                else
                {
                    _clients.Enqueue(client);
                }
            }
        }
        public Task<List<CciModuleSource>> GetWhiteModulesAsyncOld()
        {
            return Task.WhenAll(_whiteCaches.Select(wc => GetWhiteSourceAsync(_fileNameToModuleName[wc.Key])))
                .ContinueWith(t => t.Result.ToList());

        }

        public Task<List<CciModuleSource>> GetWhiteModulesAsync()
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


        public void Pause(bool pause)
        {
            lock (this)
            {
                if (!pause)
                {
                    _maxCount = _initialMaxCount;
                    _log.Debug("Whitecache unpausing. Current count: "+ _initialMaxCount);
                    Pulse();
                }
                else
                {
                    _maxCount = 1;
                    _log.Debug("Whitecache paused. Current count: " + _maxCount);
                }
            }
        }

        public void ReturnToCache(string moduleName, CciModuleSource source)
        {
            var whiteCach = _whiteCaches[_moduleNameToFileName[moduleName]];
            whiteCach.Add(source);
        }

        public Task<CciModuleSource> GetWhiteSourceAsync(string moduleName)
        {
            
            CciModuleSource cciModuleSource = TryTake(moduleName);
            if (cciModuleSource != null)
            {
                _log.Debug("Whitecache#" + PrintCacheState(_moduleNameToFileName[moduleName]) + "Taken immediately: "+ cciModuleSource.Guid);
                return Task.FromResult(cciModuleSource);
            }
            else
            {
                var client = new WhiteClient(moduleName);
                if (_error != null)
                {
                    client.Tcs.SetException(_error);
                    _log.Debug("Whitecache#" + PrintCacheState(_moduleNameToFileName[moduleName]) + "Error occurred.");
                }
                else
                {
                    _clients.Enqueue(client);
                    _log.Debug("Whitecache#"+ PrintCacheState(_moduleNameToFileName[moduleName]) +"Enqueued waiting client.");
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
            lock (this)
            {
                Monitor.Pulse(this);
            }
        }
    }

}