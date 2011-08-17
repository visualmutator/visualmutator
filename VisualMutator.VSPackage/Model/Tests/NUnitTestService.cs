namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    using NUnit.Core;
    using NUnit.Core.Filters;
    using NUnit.Util;

    public class NUnitTestService
    {

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

        public void RunTests()
        {
            TestsToRun = _unitTestsVm.TestNamespaces;
            _tl.RunTests();
        }

        private void Events_RunStarting(object sender, TestEventArgs args)
        {
            _execute.OnUIThread(
                () =>
                {
                    _unitTestsVm.AreTestsRunning = true;
                    _commandRunTests.RaiseCanExecuteChanged();

                    foreach (TestTreeNode testTreeNode in _testMap.Values)
                    {
                        testTreeNode.Status = TestStatus.Running;
                    }
                });
        }

        private void Events_TestFinished(object sender, TestEventArgs args)
        {
            //  ICollection<ITest> classes = GetTestClasses(args.Test);
            //  var namespaces = classes.Select(x => x.Parent).Distinct();

            try
            {
                TestStatus status = args.Result.IsSuccess ? TestStatus.Success : TestStatus.Failure;
                TestTreeNode node = _testMap[args.Result.Test.TestName.UniqueName];

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
                    _messageBoxService.ShowError(e.ToString());
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
                    _messageBoxService.ShowError(e.ToString());
                }
            }
        }

        private void Events_SuiteFinished(object sender, TestEventArgs args)
        {
            try
            {
                TestStatus status = args.Result.IsSuccess ? TestStatus.Success : TestStatus.Failure;

                TestTreeNode node;
                if (_testMap.TryGetValue(args.Result.Test.TestName.UniqueName, out node))
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
                    _messageBoxService.ShowError(e.ToString());
                }
            }
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

        private void Events_TestLoaded(object sender, TestEventArgs args)
        {
            try
            {
                _unitTestsVm.TestNamespaces.Clear();
                _testMap.Clear();
                ICollection<ITest> classes = GetTestClasses(args.Test);
                IEnumerable<ITest> namespaces = classes.Select(x => x.Parent).Distinct();
                foreach (ITest testNamespace in namespaces)
                {
                    var ns = new TestNodeNamespace
                    {
                        Name = testNamespace.TestName.FullName,
                        Test = testNamespace,
                    };
                    ITest testNamespace1 = testNamespace;
                    foreach (ITest testClass in classes.Where(
                        c => c.Tests != null
                             && c.Tests.Count != 0 && c.Parent == testNamespace1))
                    {
                        var c = new TestNodeClass
                        {
                            Name = testClass.TestName.Name,
                            Test = testClass,
                        };

                        foreach (ITest testMethod in testClass.Tests.Cast<ITest>())
                        {
                            var m = new TestNodeMethod
                            {
                                Name = testMethod.TestName.Name,
                                Test = testMethod,
                            };
                            c.TestMethods.Add(m);
                            _testMap.Add(testMethod.TestName.UniqueName, m);
                        }
                        ns.TestClasses.Add(c);
                        _testMap.Add(testClass.TestName.UniqueName, c);
                    }
                    _unitTestsVm.TestNamespaces.Add(ns);
                    _testMap.Add(testNamespace.TestName.UniqueName, ns);
                }
                _commandRunTests.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    _messageBoxService.ShowError(e.ToString());
                }
            }

            //            
            //            var sb = new StringBuilder();
            //            Print("", args.Test, sb);
            //
            //            using (var s = new System.IO.StreamWriter(@"C:\test.txt"))
            //            {
            //                s.Write(sb.ToString());
            //            }

            //  MessageBox.Show(sb.ToString());
        }

        private ICollection<ITest> GetTestClasses(ITest test)
        {
            var list = new List<ITest>();
            GetTestClassesInternal(list, test);
            return list;
        }

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
                _messageBoxService.ShowError(args.Exception.ToString());
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
                _messageBoxService.ShowError(args.Exception.ToString());
            }
        }
    }
}