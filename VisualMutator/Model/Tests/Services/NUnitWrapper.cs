namespace VisualMutator.Model.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure.WpfUtils.Messages;

    using NUnit.Core;
    using NUnit.Util;

    using log4net;

    public interface INUnitWrapper
    {
        IObservable<TestResult> RunFinished { get; }

        IObservable<TestResult> TestFinished { get; }

        IObservable<ITest> TestLoaded { get; }

        IObservable<TestEventArgs> TestLoadFailed { get; }

        void LoadTests(IEnumerable<string> assemblies);

        void RunTests();

        void UnloadProject();
    }

    public class NUnitWrapper : INUnitWrapper
    {
        private readonly IMessageService _messageService;

        private TestLoader _testLoader;

        private IObservable<ITest> _testLoaded;

        private IObservable<TestEventArgs> _testLoadFailed;

        private IObservable<TestResult> _runFinished;

        private IObservable<TestResult> _testFinished;

        private IObservable<Exception> _projectLoadFailed;

        private IObservable<Exception> _projectUnloadFailed;

        private IObservable<Exception> _testReloadFailed;

        private IObservable<Exception> _testUnloadFailed;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public NUnitWrapper(IMessageService messageService)
        {
            _messageService = messageService;

            ServiceManager.Services.AddService(new SettingsService());
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new RecentFilesService());
            ServiceManager.Services.AddService(new ProjectService());
            ServiceManager.Services.AddService(new TestLoader());
            ServiceManager.Services.AddService(new AddinRegistry());
            ServiceManager.Services.AddService(new AddinManager());
            ServiceManager.Services.AddService(new TestAgency());

            _testLoader = (TestLoader)ServiceManager.Services.GetService(typeof(TestLoader));
            

            _testLoaded = Observable.FromEvent<TestEventArgs>(_testLoader.Events, "TestLoaded")
                    .Select(e => e.EventArgs.Test);


            _testFinished = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "TestFinished")
                .Select(e => e.EventArgs.Result);

           
            _runFinished = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "RunFinished")
                .Select(e => e.EventArgs.Result);

            _runFinished = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "RunFinished")
                .Select(e => e.EventArgs.Result);


            _testLoadFailed = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "TestLoadFailed")
                .Select(e => e.EventArgs);
         
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


            _testReloadFailed.Subscribe(HandleException);
            _testUnloadFailed.Subscribe(HandleException);
            _projectLoadFailed.Subscribe(HandleException);
            _projectUnloadFailed.Subscribe(HandleException);

        }

        private void HandleException(Exception e)
        {
            _messageService.ShowFatalError(e, _log);
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

        public void RunTests()
        {
            
            _testLoader.RunTests();
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

        public IObservable<TestEventArgs> TestLoadFailed
        {
            get
            {
                return _testLoadFailed;
            }
        }

     
    }
}