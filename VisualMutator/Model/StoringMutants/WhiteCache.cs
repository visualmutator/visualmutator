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
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.Ast;
    using Mutations.Types;

    public class WhiteCache : IDisposable, IWhiteCache
    {
        private readonly BlockingCollection<CciModuleSource> _whiteCache;
        private IList<string> _assembliesPaths;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int _lowerBound;
        private int _currentCount;
        private IDisposable _timerRisposable;

        public WhiteCache()
        {
            _whiteCache = new BlockingCollection<CciModuleSource>(20);
            _lowerBound = 8;
        }

        public void Initialize(IList<string> assembliesPaths)
        {
            _assembliesPaths = assembliesPaths;

         //   _workers = new WorkerCollection<CciModuleSource>();


            _timerRisposable = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(200))
                .Subscribe(ev =>
                {
                    if (_whiteCache.Count < _lowerBound)
                    {
                        // Task.Run(() => _whiteCache.TryAdd(CreateSource(assembliesPaths)));
                        _whiteCache.TryAdd(CreateSource(assembliesPaths));
                    }
                     
                });
        }
        
        public CciModuleSource GetWhiteModules()
        {
            return _whiteCache.Take();

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
            if (_timerRisposable != null)
            {
                _timerRisposable.Dispose();
            }
        }
    }
}