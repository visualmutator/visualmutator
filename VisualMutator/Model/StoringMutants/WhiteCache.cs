using System.Threading.Tasks;

namespace VisualMutator.Model.StoringMutants
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.Ast;
    using Mutations.Types;

    public class WhiteCache
    {
        private readonly BlockingCollection<ModuleSource> _whiteCache;
        private IList<string> _assembliesPaths;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public WhiteCache()
        {
            _whiteCache = new BlockingCollection<ModuleSource>(4);
        }

        public void Initialize(IList<string> assembliesPaths)
        {
            _assembliesPaths = assembliesPaths;
            Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(200))
                  .Subscribe(ev =>
                  {
                      if(_whiteCache.Count < _whiteCache.BoundedCapacity)
                      {
                          var moduleSource = new ModuleSource();
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
                          _whiteCache.TryAdd(moduleSource);
                      }
                     
                  });
        }

        public ModuleSource GetWhiteModules()
        {
            return _whiteCache.Take();

        }

    }
}