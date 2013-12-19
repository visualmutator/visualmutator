namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.AccessControl;
    using System.Threading.Tasks;
    using log4net;
    using NUnit.Core;
    using NUnit.Core.Filters;
    using NUnit.Framework;
    using NUnit.Util;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Threading;

    #endregion

    public interface INUnitWrapper
    {
        IObservable<TestResult> RunFinished { get; }

        IObservable<TestResult> TestFinished { get; }

        IObservable<ITest> TestLoaded { get; }

        IObservable<Exception> TestLoadFailed { get; }
        NameFilter NameFilter { get; }

        void LoadTests(IEnumerable<string> assemblies);

        void RunTests();

        void UnloadProject();

        void Cancel();
        void CreateFilter(ICollection<TestName> names);
    }

    public class NUnitWrapper : INUnitWrapper
    {
        private readonly IMessageService _messageService;
        private readonly IDispatcherExecute _execute;

        private RemoteTestRunner _testRunner;
      //  private TestLoader _testLoader;

        private ISubject<ITest> _testLoaded;

        private ISubject<Exception> _testLoadFailed;

        private ISubject<TestResult> _runFinished;

        private ISubject<TestResult> _testFinished;

        private IObservable<Exception> _projectLoadFailed;

        private IObservable<Exception> _projectUnloadFailed;

        private IObservable<Exception> _testReloadFailed;

        private IObservable<Exception> _testUnloadFailed;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private NameFilter _nameFilter;//nullable

        public NameFilter NameFilter
        {
            get { return _nameFilter; }
        }

        public NUnitWrapper(IMessageService messageService, IDispatcherExecute execute)
        {
            _messageService = messageService;
            _execute = execute;

            InternalTrace.Initialize("nunit-visual-mutator.log", InternalTraceLevel.Verbose);
         
            CoreExtensions.Host.InitializeService();
//            ServiceManager.Services.AddService(new SettingsService());
//            ServiceManager.Services.AddService(new DomainManager());
//            ServiceManager.Services.AddService(new RecentFilesService());
//            ServiceManager.Services.AddService(new ProjectService());
//            ServiceManager.Services.AddService(new AddinRegistry());
//            ServiceManager.Services.AddService(new AddinManager());
//            ServiceManager.Services.AddService(new TestAgency());

           // _testLoader = new TestLoader();
            _testRunner = new RemoteTestRunner();
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
            _runFinished = new Subject<TestResult>();
            _testFinished = new Subject<TestResult>();
        }

        private void HandleException(Exception e)
        {
            _messageService.ShowFatalError(e);
        }
        
        public void LoadTests(IEnumerable<string> assemblies)
        {

            try
            {
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
            _testRunner.CancelRun();
           // _testLoader.CancelTestRun();
        }

        public void CreateFilter(ICollection<TestName> names)
        {
            NameFilter nameFilter = new NameFilter();
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
        public void RunTests()
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
            CustomEventListener listener = new CustomEventListener();
            listener.RunFinishedNormally.Subscribe(result =>
            {

            },
                exc =>
                {
                    _runFinished.OnError(exc);
                });
            TestResult testResult = _testRunner.Run(listener, _nameFilter, true, LoggingThreshold.All);
            IEnumerable<TestResult> selectManyRecursive = ConvertToListOf<TestResult>(testResult.Results)
                       .SelectManyRecursive(test => ConvertToListOf<TestResult>(test.Results));

            foreach (var result in selectManyRecursive)
            {
                _testFinished.OnNext(result);
            }
            _testFinished.OnCompleted();
            _runFinished.OnNext(testResult);
            _runFinished.OnCompleted();

          //  TestResult testResult = ;
           // _testLoader.RunTests(_nameFilter);
            
        }

        public void UnloadProject()
        {
            _testRunner.Unload();
           // _testLoader.UnloadProject();
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

        class CustomEventListener : EventListener
        {
            public CustomEventListener()
            {
            _runFinished = new Subject<TestResult>();
            }
            private Subject<TestResult> _runFinished;
            private Subject<Exception> _unhandled;

            public IObservable<TestResult> RunFinishedNormally
            {
                get { return _runFinished; }
            }


            public IObservable<Exception> Unhandled
            {
                get { return _unhandled; }
            }

            public void RunStarted(string name, int testCount)
            {
            }

            public void RunFinished(TestResult result)
            {
                _runFinished.OnNext(result);
                _runFinished.OnCompleted();
                
            }

            public void RunFinished(Exception exception)
            {
                _runFinished.OnError(exception);
                _runFinished.OnCompleted();
            }

            public void TestStarted(TestName testName)
            {
            }

            public void TestFinished(TestResult result)
            {
            }

            public void SuiteStarted(TestName testName)
            {
            }

            public void SuiteFinished(TestResult result)
            {
            }

            public void UnhandledException(Exception exception)
            {
                _runFinished.OnError(exception);
                _runFinished.OnCompleted();
            }

            public void TestOutput(TestOutput testOutput)
            {
            }
        }
    }
}