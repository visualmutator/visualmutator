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
           /* ModuleSource item;
            if(_whiteCache.TTake(out item, TimeSpan.FromMilliseconds(50)))
            {
                return item;
            }
            else
            {
                var moduleSource = new ModuleSource();
                foreach (var assembliesPath in assembliesPaths)
                {
                    moduleSource.AppendFromFile(assembliesPath);
                }
                return 
            }*/
        }

/*
        var results = new BlockingCollection<double>();
        var watch = Stopwatch.StartNew();
        List<Task> tasks = new List<Task>();

        var consume = Task.Factory.StartNew(() => display(results));

        for (int i = 2; i < 20; i++)
        {
            int j = i;
            var compute = Task.Factory.StartNew(() =>
            {
                results.Add(SumRootN(j));
            });
            tasks.Add(compute);
        }

        Task.Factory.ContinueWhenAll(tasks.ToArray(),
            result =>
            {
                results.CompleteAdding();
                var time = watch.ElapsedMilliseconds;
                label1.Content += time.ToString();
            }, CancellationToken.None, TaskContinuationOptions.None, ui);
    }

    public void display(BlockingCollection<double> results)
    {
        foreach (var item in results.GetConsumingEnumerable())
        {
            double currentItem = item;
            Task.Factory.StartNew(new Action(() =>
                 textBlock1.Text += currentItem.ToString() + Environment.NewLine),
            CancellationToken.None, TaskCreationOptions.None, ui);
        }
    }
*/
    }
}