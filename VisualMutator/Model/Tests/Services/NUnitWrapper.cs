namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using NUnit.Core;
    using NUnit.Core.Filters;
    using NUnit.Util;
    using UsefulTools.Core;

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

        private TestLoader _testLoader;

        private IObservable<ITest> _testLoaded;

        private IObservable<Exception> _testLoadFailed;

        private IObservable<TestResult> _runFinished;

        private IObservable<TestResult> _testFinished;

        private IObservable<Exception> _projectLoadFailed;

        private IObservable<Exception> _projectUnloadFailed;

        private IObservable<Exception> _testReloadFailed;

        private IObservable<Exception> _testUnloadFailed;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private NameFilter _nameFilter;

        public NameFilter NameFilter
        {
            get { return _nameFilter; }
        }

        public NUnitWrapper(IMessageService messageService)
        {
            _messageService = messageService;

            InternalTrace.Initialize("nunit-visual-mutator.log", InternalTraceLevel.Verbose);
            var package = new TestPackage("");
            package.Settings["RuntimeFramework"] = new RuntimeFramework(RuntimeType.Net, Environment.Version);

            ServiceManager.Services.AddService(new SettingsService());
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new RecentFilesService());
            ServiceManager.Services.AddService(new ProjectService());
            ServiceManager.Services.AddService(new AddinRegistry());
            ServiceManager.Services.AddService(new AddinManager());
            ServiceManager.Services.AddService(new TestAgency());

            _testLoader = new TestLoader(); 


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

        }

        private void HandleException(Exception e)
        {
            _messageService.ShowFatalError(e);
        }
        
        public void LoadTests(IEnumerable<string> assemblies)
        {
         
            _testLoader.NewProject();
            foreach (string project in assemblies)
            {
                _testLoader.TestProject.ActiveConfig.Assemblies.Add(project);
            }
            _testLoader.LoadTest();

        }

        public void Cancel()
        {
            _testLoader.CancelTestRun();
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

        public void RunTests()
        {
            _testLoader.RunTests(_nameFilter);
        }

        public void UnloadProject()
        {
            _testLoader.UnloadProject();
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

     
    }
}