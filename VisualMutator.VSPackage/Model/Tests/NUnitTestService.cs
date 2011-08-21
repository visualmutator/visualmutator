namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using NUnit.Core;
    using NUnit.Core.Filters;
    using NUnit.Util;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;

    #endregion

    public class NUnitTestService : ITestService
    {
        private readonly IMessageService _messageService;

        private readonly TestLoader _tl;

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

            _tl = new TestLoader();
            _tl.Events.ProjectLoadFailed += Events_ProjectLoadFailed;
            _tl.Events.ProjectLoaded += Events_ProjectLoaded;
            _tl.Events.TestLoadFailed += Events_TestLoadFailed;
            _tl.Events.TestLoaded += Events_TestLoaded;
            _tl.Events.TestFinished += Events_TestFinished;
            _tl.Events.SuiteFinished += Events_SuiteFinished;
            _tl.Events.RunFinished += Events_RunFinished;
            _tl.Events.RunStarting += Events_RunStarting;
        }

        public IDictionary<string, TestTreeNode> TestMap { get; set; }

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

        public void RefreshTestList(IEnumerable<string> assemblies)
        {
            _tl.NewProject();
            foreach (string project in assemblies)
            {
                _tl.TestProject.ActiveConfig.Assemblies.Add(project);
            }

            _tl.LoadTest();
        }

        public Task<IEnumerable<TestNodeNamespace>> LoadTests(IEnumerable<string> assemblies)
        {
            return new Task<IEnumerable<TestNodeNamespace>>(
                () =>
                {
                    _tl.NewProject();
                    foreach (string project in assemblies)
                    {
                        _tl.TestProject.ActiveConfig.Assemblies.Add(project);
                    }
                    IEnumerable<TestNodeNamespace> tests = null;
                    using (var d = new MyClass(this))
                    {
                        d.Subscribe(arg => tests = Events_TestLoaded(arg));

                        _tl.LoadTest();
                    }
                    return tests;
                });
        }

        public void RunTests()
        {
            _tl.RunTests();
        }

        private void Events_RunStarting(object sender, TestEventArgs args)
        {
        }

        private void Events_TestFinished(object sender, TestEventArgs args)
        {
            //  ICollection<ITest> classes = GetTestClasses(args.Test);
            //  var namespaces = classes.Select(x => x.Parent).Distinct();

            try
            {
                TestStatus status = args.Result.IsSuccess ? TestStatus.Success : TestStatus.Failure;
                TestTreeNode node = TestMap[args.Result.Test.TestName.UniqueName];

                node.Status = status;

                node.Result = args.Result;

                using (StreamWriter s = File.AppendText(@"C:\test.txt"))
                {
                    s.WriteLine(
                        "TestFinished   " + args.Result.Name + "  s?: " + args.Result.IsSuccess);
                }
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    _messageService.ShowError(e.ToString());
                }
            }
        }

        private void Events_RunFinished(object sender, TestEventArgs args)
        {
            try
            {
                _execute.OnUIThread(
                    () =>
                    {
                        _unitTestsVm.AreTestsRunning = false;
                        _commandRunTests.RaiseCanExecuteChanged();
                    });

                using (StreamWriter s = File.AppendText(@"C:\test.txt"))
                {
                    s.WriteLine(
                        "RunFinished   " + args.Result.Name + "  s?: " + args.Result.IsSuccess);
                }
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    _messageService.ShowError(e.ToString());
                }
            }
        }

        private void Events_SuiteFinished(object sender, TestEventArgs args)
        {
            try
            {
                TestStatus status = args.Result.IsSuccess ? TestStatus.Success : TestStatus.Failure;

                TestTreeNode node;
                if (TestMap.TryGetValue(args.Result.Test.TestName.UniqueName, out node))
                {
                    node.Status = status;
                }

                using (StreamWriter s = File.AppendText(@"C:\test.txt"))
                {
                    s.WriteLine(
                        "SuiteFinished   " + args.Result.Name + "  s?: " + args.Result.IsSuccess);
                }
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    _messageService.ShowError(e.ToString());
                }
            }
        }

        private IEnumerable<TestNodeNamespace> Events_TestLoaded(TestEventArgs args)
        {
            var list = new List<TestNodeNamespace>();
            IEnumerable<ITest> classes = GetTestClasses(args.Test).ToList();
            IEnumerable<ITest> namespaces = classes.Select(x => x.Parent).Distinct();
            foreach (ITest testNamespace in namespaces)
            {
                var ns = new TestNodeNamespace
                {
                    Name = testNamespace.TestName.FullName,
                };
                ITest testNamespace1 = testNamespace;
                foreach (ITest testClass in classes.Where(
                    c => c.Tests != null
                         && c.Tests.Count != 0 && c.Parent == testNamespace1))
                {
                    var c = new TestNodeClass
                    {
                        Name = testClass.TestName.Name,
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
                    ns.TestClasses.Add(c);
                    TestMap.Add(testClass.TestName.UniqueName, c);
                }

                TestMap.Add(testNamespace.TestName.UniqueName, ns);

                list.Add(ns);
            }
            return list;

            //  MessageBox.Show(sb.ToString());
        }

        //        private ICollection<ITest> GetTestClasses(ITest test)
        //        {
        //         
        //            var list = new List<ITest>();
        //            GetTestClassesInternal(list, test);
        //            return list;
        //        }

        private void GetTestClassesInternal(ICollection<ITest> collection, ITest test)
        {
            if (test.TestType == "TestFixture")
            {
                collection.Add(test);
                return;
            }
            else if (test.Tests != null)
            {
                foreach (ITest t in test.Tests.Cast<ITest>())
                {
                    GetTestClassesInternal(collection, t);
                }
            }
        }

        private IEnumerable<ITest> GetTestClasses(ITest test)
        {
            return test.Tests.Cast<ITest>()
                .SelectMany(GetTestClasses)
                .Where(t => t.TestType == "TestFixture");
        }

        private void Print(string pref, ITest info, StringBuilder sb)
        {
            sb.AppendLine(pref + info.TestName);
            sb.AppendLine(pref + "IsSuite: " + info.IsSuite);
            sb.AppendLine(pref + "TestType: " + info.TestType);
            sb.AppendLine(pref);
            if (info.Tests != null)
            {
                foreach (ITest test in info.Tests.Cast<ITest>())
                {
                    Print(pref + "   ", test, sb);
                }
            }
        }

        private void Events_TestLoadFailed(object sender, TestEventArgs args)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            else
            {
                _messageService.ShowError(args.Exception.ToString());
            }
        }

        private void Events_ProjectLoaded(object sender, TestEventArgs args)
        {
        }

        private void Events_ProjectLoadFailed(object sender, TestEventArgs args)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            else
            {
                _messageService.ShowError(args.Exception.ToString());
            }
        }

        #region Nested type: MyClass

        private class MyClass : IObservable<TestEventArgs>, IDisposable
        {
            private readonly NUnitTestService _service;

            private readonly IDisposable _testLoadFailed;

            private readonly IDisposable _testLoaded;

            private IObserver<TestEventArgs> _observer;

            public MyClass(NUnitTestService service)
            {
                _service = service;
         

                _testLoaded = Observable.FromEvent<TestEventArgs>(_service._tl.Events, "TestLoaded")
                    .Select(e => e.EventArgs).Subscribe(TestLoadedHandler);

                _testLoadFailed = Observable.FromEvent<TestEventArgs>(
                    _service._tl.Events, "TestLoadFailed")
                    .Select(e => e.EventArgs).Subscribe(TestLoadFailedHandler);
            }

            public void Dispose()
            {
                _testLoaded.Dispose();
                _testLoadFailed.Dispose();
            }

            public IDisposable Subscribe(IObserver<TestEventArgs> observer)
            {
                _observer = observer;
                return this;
                //   return subject.Subscribe(observer);
            }

            private void TestLoadedHandler(TestEventArgs sArgs)
            {
                try
                {
                    _observer.OnNext(sArgs);
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

        #endregion

        #region Nested type: TestsRunning

        private class TestsRunning : IObservable<TestEventArgs>, IDisposable
        {
            private readonly IDisposable _runFinished;

            private readonly NUnitTestService _service;

            private readonly IDisposable _testFinished;

            private IObserver<TestEventArgs> _observer;

            private IDisposable _suiteFinished;

        

            public TestsRunning(NUnitTestService service)
            {
                _service = service;
              

                _testFinished = Observable.FromEvent<TestEventArgs>(
                    _service._tl.Events, "TestFinished")
                    .Select(e => e.EventArgs.Result)
                    .Subscribe(TestFinishedHandler);

              
                _suiteFinished = Observable.FromEvent<TestEventArgs>(
                    _service._tl.Events, "SuiteFinished")
                    .Select(e => e.EventArgs.Result)
                    //.Where(result => _service.TestMap.ContainsKey(result.Test.TestName.UniqueName))
                    .Where(result => result.Test.TestType == "TestFixture")
                    .Subscribe(TestFinishedHandler);

                _runFinished = Observable.FromEvent<TestEventArgs>(
                       _service._tl.Events, "RunFinished")
                       .Select(e => e.EventArgs.Result)
                       .Subscribe(RunFinishedHandler);

            }

            public void Dispose()
            {
                _testFinished.Dispose();
                _runFinished.Dispose();
                _suiteFinished.Dispose();
            }

            public IDisposable Subscribe(IObserver<TestEventArgs> observer)
            {
                _observer = observer;
                return this;   
     
            }

            private void TestFinishedHandler(TestResult result)
            {
                TestStatus status = result.IsSuccess ? TestStatus.Success : TestStatus.Failure;
                TestTreeNode node = _service.TestMap[result.Test.TestName.UniqueName];

                node.Status = status;

                node.Result = result;
            }

            private void RunFinishedHandler(TestResult sArgs)
            {
                       
            }
          
        }

        #endregion
    }
}