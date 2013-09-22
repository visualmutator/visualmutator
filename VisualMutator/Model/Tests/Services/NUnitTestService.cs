namespace VisualMutator.Model.Tests.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;

    using NUnit.Core;
    using NUnit.Util;

    using VisualMutator.Model.Exceptions;
    using VisualMutator.Model.Tests.TestsTree;

    using log4net;

    #endregion

    public class NUnitTestService : ITestService
    {
        private readonly INUnitWrapper _nUnitWrapper;

        private readonly IMessageService _messageService;

        private bool _currentRunCancelled;
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public NUnitTestService(INUnitWrapper nUnitWrapper, IMessageService messageService)
        {
            _nUnitWrapper = nUnitWrapper;
            _messageService = messageService;

        }

        public INUnitWrapper TestLoader
        {
            get
            {
                return _nUnitWrapper;
            }
        }


        public IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies, MutantTestSession mutantTestSession)
        {
            _currentRunCancelled = false;
            Exception exc = null;
            IEnumerable<TestNodeClass> tests = null;
            using (var eventObj = new ManualResetEventSlim())
            using (var job = new TestsLoadJob(this, assemblies))
            {
                var sw = new Stopwatch();
                sw.Start();
                job.Subscribe(arg =>
                {
                    sw.Stop();
                    mutantTestSession.LoadTestsTimeRawMiliseconds = sw.ElapsedMilliseconds;
                    tests = BuildTestTree(arg, mutantTestSession);
                },
                ex =>
                    {
                        throw new Exception("Exception occurred while loading tests. ", ex);
                    },
                onCompleted: eventObj.Set);
                eventObj.Wait();
            }
            if (tests == null)
            {
                throw new Exception("tests == null");
            }
            if (exc != null)
            {
                throw new Exception("Exception occurred while loading tests. ",exc);
            }

            return tests.ToList();
        }

        public List<TestNodeMethod> RunTests(MutantTestSession mutantTestSession)
        {

            var list = new List<TestNodeMethod>();
            using (var eventObj = new ManualResetEventSlim())
            using (var job = new TestsRunJob(this))
            {
                var sw = new Stopwatch();
                sw.Start();
                job.Subscribe(result =>
                {
                    TestNodeMethod node = mutantTestSession.TestMap[result.Test.TestName.UniqueName];
                    node.State = result.IsSuccess ? TestNodeState.Success : TestNodeState.Failure;
                    node.Message = result.Message;
                    list.Add(node);
                },
                onError: ex => _messageService.ShowFatalError(ex, _log),
                onCompleted: eventObj.Set);
                eventObj.Wait();
                sw.Stop();

                mutantTestSession.RunTestsTimeRawMiliseconds = sw.ElapsedMilliseconds;

                if (_currentRunCancelled)
                {
                    throw new TestingCancelledException();
                }

            }
            return list;
        }

        public void UnloadTests()
        {
            
            _nUnitWrapper.UnloadProject();
        }

        private IEnumerable<TestNodeClass> BuildTestTree(ITest test, MutantTestSession mutantTestSession)
        {
            var list = new List<TestNodeClass>();
            IEnumerable<ITest> classes = GetTestClasses(test).ToList();

            foreach (ITest testClass in classes.Where(c => c.Tests != null && c.Tests.Count != 0))
            {
                
                var c = new TestNodeClass(testClass.TestName.Name)
                {
                    Namespace = testClass.Parent.TestName.FullName,
                    FullName = testClass.TestName.FullName,

                };

                foreach (ITest testMethod in testClass.Tests.Cast<ITest>())
                {
                    if (_nUnitWrapper.NameFilter == null || _nUnitWrapper.NameFilter.Match(testMethod))
                    {
                        var m = new TestNodeMethod(c, testMethod.TestName.Name);
                        m.TestId = new NUnitTestId(testMethod.TestName);
                        c.Children.Add(m);
                        mutantTestSession.TestMap.Add(testMethod.TestName.UniqueName, m);
                    }
                   
                }
                if(c.Children.Any())
                {
                    list.Add(c);
                }
            }
            return list;
        }

        private IEnumerable<ITest> GetTestClasses(ITest test)
        {
           //TODO: return new[] { test }.SelectManyRecursive(t => t.Tests != null ? t.Tests.Cast<ITest>() : new ITest[0])
           //     .Where(t => t.TestType == "TestFixture");
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
                foreach (var t in tests.Cast<ITest>())
                {
                    GetTestClassesInternal(list, t);
                }
            }
        }


        public void Cancel()
        {
            _currentRunCancelled = true;
            _nUnitWrapper.Cancel();
        }

        public void CreateTestFilter(ICollection<TestId> selectedTests)
        {
            _nUnitWrapper.CreateFilter(selectedTests.Cast<NUnitTestId>().Select(id =>id.TestName).ToList());
        }


        private class TestsLoadJob : IObservable<ITest>, IDisposable
        {
            private readonly NUnitTestService _service;

            private IDisposable _testLoadFailed;

            private IDisposable _testLoaded;

            private IEnumerable<string> _assemblies;

            private IObserver<ITest> _observer;

            public TestsLoadJob(NUnitTestService service, IEnumerable<string> assemblies)
            {
                _service = service;
                _assemblies = assemblies;
            }

            public void Dispose()
            {
                _testLoaded.Dispose();
                _testLoadFailed.Dispose();
            }

            public IDisposable Subscribe(IObserver<ITest> observer)
            {
                _observer = observer;
                _testLoaded = _service.TestLoader.TestLoaded.Subscribe(TestLoadedHandler);
                _testLoadFailed = _service.TestLoader.TestLoadFailed.Subscribe(TestLoadFailedHandler);
                _service.TestLoader.LoadTests(_assemblies);
                return this;
            }

            private void TestLoadedHandler(ITest test)
            {
                try
                {
                    _observer.OnNext(test);
                    _observer.OnCompleted();
                }
                catch (Exception e)
                {
                    _observer.OnError(e);
                }
            }

            private void TestLoadFailedHandler(Exception sArgs)
            {
                _observer.OnError(sArgs);
            }


        }





        private class TestsRunJob : IObservable<TestResult>, IDisposable
        {
            private IDisposable _runFinished;

            private readonly NUnitTestService _service;

            private IDisposable _testFinished;

            private IObserver<TestResult> _observer;

            public TestsRunJob(NUnitTestService service)
            {
                _service = service;

            }

            public void Dispose()
            {
                _testFinished.Dispose();
                _runFinished.Dispose();
               
            }

            public IDisposable Subscribe(IObserver<TestResult> observer)
            {
                _observer = observer;

                _testFinished = _service.TestLoader.TestFinished.Subscribe(TestFinished);
               
                _runFinished = _service.TestLoader.RunFinished.Subscribe(RunFinished);
                _service.TestLoader.RunTests();
                return this;
            }

            private void TestFinished(TestResult result)
            {

                _observer.OnNext(result);
            }

            private void RunFinished(TestResult result)
            {
                _observer.OnCompleted();
            }
        }

    }
}