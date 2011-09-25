namespace VisualMutator.Model.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Core;
    using NUnit.Util;

    public interface INUnitWrapper
    {
        IObservable<TestResult> RunFinished { get; }

        IObservable<TestResult> TestFinished { get; }

        IObservable<ITest> TestLoaded { get; }

        IObservable<TestEventArgs> TestLoadFailed { get; }

        void LoadTests(IEnumerable<string> assemblies);

        void RunTests();
    }

    public class NUnitWrapper : INUnitWrapper
    {
        private TestLoader _testLoader;

        private IObservable<ITest> _testLoaded;

        private IObservable<TestEventArgs> _testLoadFailed;

        private IObservable<TestResult> _runFinished;

        private IObservable<TestResult> _testFinished;

        public NUnitWrapper()
        {
            
            ServiceManager.Services.AddService(new SettingsService());
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new RecentFilesService());
            ServiceManager.Services.AddService(new ProjectService());
            ServiceManager.Services.AddService(new TestLoader());
            ServiceManager.Services.AddService(new AddinRegistry());
            ServiceManager.Services.AddService(new AddinManager());
            ServiceManager.Services.AddService(new TestAgency());

            _testLoader = new TestLoader();


            _testLoaded = Observable.FromEvent<TestEventArgs>(_testLoader.Events, "TestLoaded")
                    .Select(e => e.EventArgs.Test);

            _testLoadFailed = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "TestLoadFailed")
                .Select(e => e.EventArgs);


            _testFinished = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "TestFinished")
                .Select(e => e.EventArgs.Result);
            /*
            _suiteFinished = Observable.FromEvent<TestEventArgs>(
                _service.TestLoader.Events, "SuiteFinished")
                .Select(e => e.EventArgs.Result)
                .Where(result => result.Test.TestType == "TestFixture")
                .Subscribe(_observer.OnNext);
            */
            _runFinished = Observable.FromEvent<TestEventArgs>(
                _testLoader.Events, "RunFinished")
                .Select(e => e.EventArgs.Result);

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