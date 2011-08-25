namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using NUnit.Core;
    using NUnit.Core.Filters;
    using NUnit.Util;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;

    #endregion

    public class NUnitTestService : AbstractTestService
    {
        private readonly IMessageService _messageService;

        private readonly TestLoader _testLoader;

        public NUnitTestService(IMessageService messageService)
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

            _testLoader = new TestLoader();
        }

        public TestLoader TestLoader
        {
            get
            {
                return _testLoader;
            }
        }

        private TestFilter MakeNameFilter(ICollection<ITest> tests)
        {
            if (tests == null || tests.Count == 0)
            {
                return TestFilter.Empty;
            }

            var nameFilter = new NameFilter();
            foreach (ITest test in tests)
            {
                nameFilter.Add(test.TestName);
            }

            return nameFilter;
        }

        public override IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies)
        {
            TestMap.Clear();
            IEnumerable<TestNodeClass> tests = null;
            using (var job = new TestsLoadJob(this, assemblies))
            {
                job.Subscribe(arg => tests = BuildTestTree(arg));
            }
            return tests;
        }

        public override void RunTests()
        {
            TestMap.Values.Each(t => t.Status = TestStatus.Running);

            using (var eventObj = new ManualResetEventSlim())
            {
                using (var job = new TestsRunJob(this))
                {
                    job.Subscribe(result =>
                    {
                        TestTreeNode node = TestMap[result.Test.TestName.UniqueName];
                        node.Status = result.IsSuccess ? TestStatus.Success : TestStatus.Failure;
                        node.Message = result.Message;
                    }, () => { eventObj.Set(); });
                    eventObj.Wait();
                }
            }
        }

        private IEnumerable<TestNodeClass> BuildTestTree(ITest test)
        {
            var list = new List<TestNodeClass>();
            IEnumerable<ITest> classes = GetTestClasses(test).ToList();

            foreach (ITest testClass in classes.Where(c => c.Tests != null
                                                           && c.Tests.Count != 0))
            {
                var c = new TestNodeClass
                {
                    Name = testClass.TestName.Name,
                    Namespace = testClass.Parent.TestName.FullName
                };

                foreach (ITest testMethod in testClass.Tests.Cast<ITest>())
                {
                    var m = new TestNodeMethod
                    {
                        Name = testMethod.TestName.Name,
                    };
                    c.TestMethods.Add(m);
                    TestMap.Add(testMethod.TestName.UniqueName, m);
                }

                TestMap.Add(testClass.TestName.UniqueName, c);
                list.Add(c);
            }
            return list;
        }

        private IEnumerable<ITest> GetTestClasses(ITest test)
        {
            var list = new List<ITest>();
            GetTestClassesInternal(list, test);
            return list;
        }

        private void GetTestClassesInternal(List<ITest> list, ITest test)
        {
            var tests = test.Tests ?? new ITest[0];
            if (test.TestType == "TestFixture")
            {
                list.Add(test);
            }
            else
            {
                tests.Cast<ITest>().Each(t => GetTestClassesInternal(list, t));
            }
        }

        private class TestsLoadJob : IObservable<ITest>, IDisposable
        {
            private readonly NUnitTestService _service;

            private readonly IDisposable _testLoadFailed;

            private readonly IDisposable _testLoaded;

            private IEnumerable<string> _assemblies;

            private IObserver<ITest> _observer;

            public TestsLoadJob(NUnitTestService service, IEnumerable<string> assemblies)
            {
                _service = service;
                _assemblies = assemblies;

                _testLoaded = Observable.FromEvent<TestEventArgs>(_service.TestLoader.Events, "TestLoaded")
                    .Select(e => e.EventArgs).Subscribe(TestLoadedHandler);

                _testLoadFailed = Observable.FromEvent<TestEventArgs>(
                    _service.TestLoader.Events, "TestLoadFailed")
                    .Select(e => e.EventArgs).Subscribe(TestLoadFailedHandler);
            }

            public void Dispose()
            {
                _testLoaded.Dispose();
                _testLoadFailed.Dispose();
            }

            public IDisposable Subscribe(IObserver<ITest> observer)
            {
                _observer = observer;

                _service.TestLoader.NewProject();
                foreach (string project in _assemblies)
                {
                    _service.TestLoader.TestProject.ActiveConfig.Assemblies.Add(project);
                }

                _service.TestLoader.LoadTest();
                return this;
            }

            private void TestLoadedHandler(TestEventArgs sArgs)
            {
                try
                {
                    _observer.OnNext(sArgs.Test);
                }
                catch (Exception e)
                {
                    _service._messageService.ShowError(e);
                }
            }

            private void TestLoadFailedHandler(TestEventArgs sArgs)
            {
                _service._messageService.ShowError(sArgs.Exception);
                //                try
                //                {
                //                    _observer.OnError(sArgs.Exception);
                //                }
                //                catch (Exception e)
                //                {
                //                    
                //
                //                    throw;
                //                }
                //                
            }
        }

        private class TestsRunJob : IObservable<TestResult>, IDisposable
        {
            private readonly IDisposable _runFinished;

            private readonly NUnitTestService _service;

            private readonly IDisposable _testFinished;

            private IObserver<TestResult> _observer;

            private IDisposable _suiteFinished;

            public TestsRunJob(NUnitTestService service)
            {
                _service = service;

                _testFinished = Observable.FromEvent<TestEventArgs>(
                    _service.TestLoader.Events, "TestFinished")
                    .Select(e => e.EventArgs.Result)
                    .Subscribe(TestFinishedHandler);

                _suiteFinished = Observable.FromEvent<TestEventArgs>(
                    _service.TestLoader.Events, "SuiteFinished")
                    .Select(e => e.EventArgs.Result)
                    .Where(result => result.Test.TestType == "TestFixture")
                    .Subscribe(TestFinishedHandler);

                _runFinished = Observable.FromEvent<TestEventArgs>(
                    _service.TestLoader.Events, "RunFinished")
                    .Select(e => e.EventArgs.Result)
                    .Subscribe(RunFinishedHandler);
            }

            public void Dispose()
            {
                _testFinished.Dispose();
                _runFinished.Dispose();
                _suiteFinished.Dispose();
            }

            public IDisposable Subscribe(IObserver<TestResult> observer)
            {
                _observer = observer;
                _service.TestLoader.RunTests();
                return this;
            }

            private void TestFinishedHandler(TestResult result)
            {
                _observer.OnNext(result);
            }

            private void RunFinishedHandler(TestResult sArgs)
            {
                _observer.OnCompleted();
            }
        }
    }
}