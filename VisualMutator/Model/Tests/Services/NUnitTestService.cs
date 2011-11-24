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

    using VisualMutator.Model.Tests.TestsTree;

    using log4net;

    #endregion

    public class NUnitTestService : ITestService
    {
        private readonly INUnitWrapper _nUnitWrapper;

        private readonly IMessageService _messageService;

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


        public IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies, TestSession testSession)
        {
       
            IEnumerable<TestNodeClass> tests = null;
            using (var job = new TestsLoadJob(this, assemblies))
            {
                var sw = new Stopwatch();
                sw.Start();
                job.Subscribe(arg =>
                {
                    sw.Stop();
                    testSession.LoadTestsTimeRawMiliseconds = sw.ElapsedMilliseconds;
                    tests = BuildTestTree(arg, testSession);
                },
                    ex => _messageService.ShowFatalError(ex,_log));
            }

            if (tests == null)
            {
                throw new InvalidOperationException();
            }

            return tests;
        }

        public List<TestNodeMethod> RunTests(TestSession testSession)
        {
            var list = new List<TestNodeMethod>();
            using (var eventObj = new ManualResetEventSlim())
            using (var job = new TestsRunJob(this))
            {
                job.Subscribe(result =>
                {
                    TestNodeMethod node = testSession.TestMap[result.Test.TestName.UniqueName];
                    node.State = result.IsSuccess ? TestNodeState.Success : TestNodeState.Failure;
                    node.Message = result.Message;
                    list.Add(node);
                },
                onError: ex => _messageService.ShowFatalError(ex, _log),
                onCompleted: eventObj.Set);

                var sw = new Stopwatch();
                sw.Start();
                eventObj.Wait();
                sw.Stop();

                testSession.RunTestsTimeRawMiliseconds = sw.ElapsedMilliseconds;


            }
            return list;
        }

        public void UnloadTests()
        {
            
            _nUnitWrapper.UnloadProject();
        }

        private IEnumerable<TestNodeClass> BuildTestTree(ITest test, TestSession testSession)
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
                    var m = new TestNodeMethod(c, testMethod.TestName.Name);
                    c.Children.Add(m);
                    testSession.TestMap.Add(testMethod.TestName.UniqueName, m);
                }

                //TestMap.Add(testClass.TestName.UniqueName, c);
                list.Add(c);
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
            _nUnitWrapper.Cancel();
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
                }
                catch (Exception e)
                {
                    _service._messageService.ShowFatalError(e, _service._log);
                }
            }

            private void TestLoadFailedHandler(TestEventArgs sArgs)
            {
                _service._messageService.ShowFatalError(sArgs.Exception, _service._log);
            
            }


        }





        private class TestsRunJob : IObservable<TestResult>, IDisposable
        {
            private IDisposable _runFinished;

            private readonly NUnitTestService _service;

            private IDisposable _testFinished;

            private IObserver<TestResult> _observer;
 //  private IDisposable _suiteFinished;
         

            public TestsRunJob(NUnitTestService service)
            {
                _service = service;

            }

            public void Dispose()
            {
                _testFinished.Dispose();
                _runFinished.Dispose();
                //  _suiteFinished.Dispose();
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