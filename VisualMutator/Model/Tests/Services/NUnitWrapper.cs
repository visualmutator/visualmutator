namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.AccessControl;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using NUnit.Core;
    using NUnit.Core.Filters;
    using NUnit.Framework;
    using NUnit.Util;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using UsefulTools.Threading;

    #endregion

    public interface INUnitWrapper
    {
        IObservable<TestResult> RunFinished { get; }

        IObservable<TestResult> TestFinished { get; }

        IObservable<ITest> TestLoaded { get; }

        IObservable<Exception> TestLoadFailed { get; }
        TestFilter NameFilter { get; }

        void LoadTests(IEnumerable<string> assemblies);

        Task<TestResult> RunTests();

        void UnloadProject();

        void Cancel();
        void CreateFilter(ICollection<TestName> names);
         NUnitWrapper.CustomEventListener Listener
        {
            get;
        }
    }

    public class NUnitWrapper : INUnitWrapper
    {
        private readonly IMessageService _messageService;

        private TestRunner _testRunner;
      //  private TestLoader _testLoader;

        private readonly ISubject<ITest> _testLoaded;

        private readonly ISubject<Exception> _testLoadFailed;

        private readonly ISubject<TestResult> _runFinished;

        private readonly ISubject<TestResult> _testFinished;

     

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private TestFilter _nameFilter;//nullable
        private MyRecordingListener listener;

        public CustomEventListener Listener
        {
            get { return null; }
        }

        public TestFilter NameFilter
        {
            get { return _nameFilter; }
        }

        public NUnitWrapper(IMessageService messageService)
        {
            _messageService = messageService;

            InternalTrace.Initialize("nunit-visual-mutator.log", InternalTraceLevel.Verbose);
         
            CoreExtensions.Host.InitializeService();
            ServiceManager.Services.AddService(new SettingsService());
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new RecentFilesService());
            ServiceManager.Services.AddService(new ProjectService());
            ServiceManager.Services.AddService(new AddinRegistry());
            ServiceManager.Services.AddService(new AddinManager());
            ServiceManager.Services.AddService(new TestAgency());

           // _testLoader = new TestLoader();
            _testRunner = new ProcessRunner();
            _testRunner.Unload();
            
            /*
            _testLoaded = Observable.FromEvent<TestEventArgs>(_testLoader.Events, "TestLoaded")
               // .Where(e => e.EventArgs.Test != null)
                .Select(e => e.EventArgs.Test);
             

            _testLoadFailed = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "TestLoadFailed")
                .Select(e => e.EventArgs.Exception);
         
            _testReloadFailed = Observable.FromEvent<TestEventArgs>(
              _testLoader.Events, "TestReloadFailed")
              .Select(e => e.EventArgs.Exception);            
            
            _testUnloadFailed = Observable.FromEvent<TestEventArgs>(
              _testLoader.Events, "TestUnloadFailed")
              .Select(e => e.EventArgs.Exception);

            _projectLoadFailed = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "ProjectLoadFailed")
                .Select(e => e.EventArgs.Exception);

            _projectUnloadFailed = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "ProjectUnloadFailed")
                .Select(e => e.EventArgs.Exception);



            _testFinished = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "TestFinished")
                .Select(e => e.EventArgs.Result);


            _runFinished = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "RunFinished")
                .Select(e => e.EventArgs.Result);

            

            _testLoadFailed.Subscribe(HandleException);
            _testReloadFailed.Subscribe(HandleException);
            _testUnloadFailed.Subscribe(HandleException);
            _projectLoadFailed.Subscribe(HandleException);
            _projectUnloadFailed.Subscribe(HandleException);
            */

            _testLoadFailed = new Subject<Exception>();
            _testLoaded = new Subject<ITest>();
            _runFinished = new ReplaySubject<TestResult>();
            _testFinished = new ReplaySubject<TestResult>();
        }

        private void HandleException(Exception e)
        {
            _messageService.ShowFatalError(e);
        }
        
        public void LoadTests(IEnumerable<string> assemblies)
        {

            try
            {
                //Assembly.
               // _testRunner = new TestDomain();

                var package = new TestPackage("", assemblies.ToList());
                package.Settings["RuntimeFramework"] = new RuntimeFramework(RuntimeType.Net, Environment.Version);
                package.Settings["UseThreadedRunner"] = false;
                bool load = _testRunner.Load(package);
                if (load)
                {
                    _testLoaded.OnNext(_testRunner.Test);
                }
                else
                {
                    _log.Warn("!load");
                    _testLoaded.OnNext(_testRunner.Test);
                }
            }
            catch (Exception e)
            {
                _testLoadFailed.OnNext(e);
                _testLoadFailed.OnCompleted();
            }
         //   _testLoader.NewProject();
//            foreach (string project in assemblies.Where(f => f.Contains("Test")))
//            {
//                _testLoader.TestProject.ActiveConfig.Assemblies.Add(project);
//            }
         //   _testLoader.LoadTest();

        }

        public void Cancel()
        {
            if (_testRunner.Running)
            {
                _testRunner.CancelRun();
            }
           // _testLoader.CancelTestRun();
        }

        public void CreateFilter(ICollection<TestName> names)
        {
            var nameFilter = new IDTestFilter();
         //   IEnumerable<ITest> selectManyRecursive = ConvertToListOf<ITest>(_testRunner.Test.Tests)
        //                .SelectManyRecursive(test => ConvertToListOf<ITest>(test.Tests));

            foreach (TestName name in names)
            {
                nameFilter.Add(name);
            }
            _nameFilter = nameFilter;
        }
        public static IList<T> ConvertToListOf<T>(IList iList)
        {
            IList<T> result = new List<T>();
            if (iList != null)
            {
                foreach (T value in iList)
                {
                    result.Add(value);
                }
            }

            return result;
        }
        public Task<TestResult> RunTests()
        {
           /* 
            Task<TestResult> task = Task.Run(() => _testRunner.Run(new NullListener(), _nameFilter, true, LoggingThreshold.All));
            task.ContinueWith(t =>
            {
                if (t.Exception == null)
                {
                    IEnumerable<TestResult> selectManyRecursive = ConvertToListOf<TestResult>(t.Result.Results)
                        .SelectManyRecursive(test => ConvertToListOf<TestResult>(test.Results));

                    foreach (var result in selectManyRecursive)
                    {
                        _testFinished.OnNext(result);
                    }
                    _testFinished.OnCompleted();
                    _runFinished.OnNext(t.Result);
                    _runFinished.OnCompleted();
                }
            });*/
            return Task.Run(() =>
            {
                var taskCompletion = new TaskCompletionSource<TestResult>();


                 listener = new MyRecordingListener();


                 /*   listener.RunFinishedNormally.Subscribe(result =>
                    {
                        IEnumerable<TestResult> selectManyRecursive = ConvertToListOf<TestResult>(result.Results)
                                   .SelectManyRecursive(test => ConvertToListOf<TestResult>(test.Results));

                        foreach (var t in selectManyRecursive)
                        {
                            _testFinished.OnNext(t);
                        }
                        _testFinished.OnCompleted();
                        _runFinished.OnNext(result);
                        _runFinished.OnCompleted();
                    
                        taskCompletion.SetResult(result);
                    },
                    exc =>
                    {
                        _runFinished.OnError(exc);
                        taskCompletion.TrySetException(exc);
                    });
                    */
                 //  _nameFilter = new EmptyFilter();
                _testRunner.BeginRun(listener, _nameFilter, true, LoggingThreshold.All);
                new Thread(() =>
                {
                    TestDomain d = (TestDomain) _testRunner;
                    d.Wait();
                    if(_testRunner.TestResult.Results != null)
                    {
                        var leafs = _testRunner.TestResult.Results.Cast<TestResult>()
                            .SelectManyRecursive(t => t.Results == null ? new List<TestResult>() :
                                t.Results.Cast<TestResult>(), leafsOnly: true);
                        foreach (var t in leafs)
                        {
                            _testFinished.OnNext(t);
                        }
                       // _testFinished.OnCompleted();
                        _runFinished.OnNext(_testRunner.TestResult);
                       // _runFinished.OnCompleted();
                    }
                    taskCompletion.SetResult(null);

                }).Start();
             //    Task.Run();
                
                return taskCompletion.Task;
            });
            
          //  TestResult testResult = ;
           // _testLoader.RunTests(_nameFilter);
            
        }

        public void UnloadProject()
        {
            FilePathAbsolute path = null;
            if (_testRunner.Test != null)
            {
                TestAssemblyInfo firstOrDefault = _testRunner.AssemblyInfo.Cast<TestAssemblyInfo>().FirstOrDefault();
                if(firstOrDefault != null)
                {
                    try
                    {
                        path = new FilePathAbsolute(firstOrDefault.Name);

                        
                            
                            
                    }
                    catch (Exception e)
                    {
                        _log.Warn(e);
                        //Console.WriteLine(e);
                    }

                }
            }
            _testRunner.Unload();
            _testRunner.Dispose();
            try
            {
                if (path != null)
                {
                    Directory.Delete(path.ParentDirectoryPath.ToString(), recursive: true);
                }
            }
            catch (Exception e)
            {
                _log.Warn(e);
            }
           // _testLoader.UnloadProject()
            ;
        }

        public IObservable<TestResult> RunFinished
        {
            get
            {
                return _runFinished;
            }
        }

        public IObservable<TestResult> TestFinished
        {
            get
            {
                return _testFinished;
            }
        }

        public IObservable<ITest> TestLoaded
        {
            get
            {
                return _testLoaded;
            }
        }

        public IObservable<Exception> TestLoadFailed
        {
            get
            {
                return _testLoadFailed;
            }
        }
        private class EmptyFilter : TestFilter
        {
            public override bool Match(ITest test)
            {
                return true;
            }

            public override bool Pass(ITest test)
            {
                return true;
            }
        }
        [Serializable]
        private class IDTestFilter : TestFilter
        {
           	private readonly ArrayList testNames = new ArrayList();

		    public IDTestFilter() { }

            public IDTestFilter(TestName testName)
		    {
			    testNames.Add( testName );
		    }

		    public void Add( TestName testName )
		    {
			    testNames.Add( testName );
		    }
            public override bool Match(ITest test)
            {
                foreach (TestName testName in testNames)
                {
                    if (test.TestName.FullName.Equals(testName.FullName))
                    {
                        return true;
                    }
                }
                return false;
            }
        }


        public class CustomEventListener : EventListener
        {
            private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            public CustomEventListener()
            {
                _runFinished = new Subject<TestResult>();
                _testFinished = new ReplaySubject<TestResult>();
            }
            private readonly Subject<TestResult> _runFinished;
            private readonly ReplaySubject <TestResult> _testFinished;
  

            public IObservable<TestResult> RunFinishedNormally
            {
                get { return _runFinished; }
            }

            public ReplaySubject<TestResult> TestFinished1
            {
                get { return _testFinished; }
            }


            public void RunStarted(string name, int testCount)
            {
                _log.Info("RunStarted: " + name);
            }

            public void RunFinished(TestResult result)
            {
                _log.Info("Run finished.");
                _testFinished.OnCompleted();
                _runFinished.OnNext(result);
                _runFinished.OnCompleted();
            }

            public void RunFinished(Exception exception)
            {
                _log.Error("Run finished with exception.");
                _testFinished.OnError(exception);
                _runFinished.OnError(exception);
            }

            public void TestStarted(TestName testName)
            {
                _log.Info("TestStarted: " + testName.Name);
            }

            public void TestFinished(TestResult result)
            {
                _log.Info("TestFinished: " + result.Name + " - " + result.IsSuccess);
                _testFinished.OnNext(result);
            }

            public void SuiteStarted(TestName testName)
            {
                _log.Info("SuiteStarted: " + testName.Name);
            }

            public void SuiteFinished(TestResult result)
            {
                _log.Info("SuiteFinished: " + result.Name + " - " + result.IsSuccess);
            }

            public void UnhandledException(Exception exception)
            {
                _log.Error("Test UnhandledException.", exception);
                _testFinished.OnError(exception);
                _runFinished.OnError(exception);
            }

            public void TestOutput(TestOutput testOutput)
            {
               // _log.Info("SuiteFinished: " + result.Name + " - " + result.IsSuccess);
            }
        }

        [Serializable]
        public class MyRecordingListener : EventListener
        {
         
            public List<TestResult> finishedTests = new List<TestResult>();

            public List<TestResult> FinishedTests
            {
                get { return finishedTests; }
            }

            public void RunStarted(string name, int testCount)
            {
            }

            public void RunFinished(NUnit.Core.TestResult result)
            {
            }

            public void RunFinished(Exception exception)
            {
            }

            public void TestStarted(TestName testName)
            {
            }

            public void TestFinished(TestResult result)
            {
                finishedTests.Add(result);
            }

            public void SuiteStarted(TestName suiteName)
            {
            }

            public void SuiteFinished(TestResult result)
            {
            }

            public void UnhandledException(Exception exception)
            {
            }

            public void TestOutput(TestOutput testOutput)
            {
            }
        }

    }
}