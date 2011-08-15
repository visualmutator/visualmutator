namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Waf.Applications;

    using NUnit.Core;
    using NUnit.Core.Filters;
    using NUnit.Util;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.Tests;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;

    using VisualMutator.Domain;

    #endregion

    public class UnitTestsController : Controller
    {
        private readonly Dictionary<string, TestTreeNode> _testMap;

        private readonly TestLoader _tl;

        private readonly UnitTestsViewModel _unitTestsVm;

        private readonly IVisualStudioConnection _visualStudioConnection;

        private readonly IMessageBoxService _messageBoxService;

        private readonly IExecute _execute;

        private ObservableCollection<MutationSession> _mutants;

        private DelegateCommand _commandRunTests;

        public UnitTestsController(
            UnitTestsViewModel unitTestsVm,
            IVisualStudioConnection visualStudioConnection,
            IMessageBoxService messageBoxService,
            IExecute execute
            )
        {
            _unitTestsVm = unitTestsVm;
            _visualStudioConnection = visualStudioConnection;
            _messageBoxService = messageBoxService;
            _execute = execute;

            _commandRunTests = new DelegateCommand(
                RunTests, () => _unitTestsVm.SelectedMutant!=null && !_unitTestsVm.AreTestsRunning );
            _unitTestsVm.CommandRunTests = _commandRunTests;


            _testMap = new Dictionary<string, TestTreeNode>();
            AddWeakEventListener(unitTestsVm, ViewModelChanged);

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
            _tl.Events.RunStarting += new TestEventHandler(Events_RunStarting);
            // _tl.Events.
        }

      
        public UnitTestsViewModel UnitTestsVm
        {
            get
            {
                return _unitTestsVm;
            }
        }

        public ObservableCollection<MutationSession> Mutants
        {
            set
            {
                _mutants = value;
                _unitTestsVm.Mutants = value;
            }
            get
            {
                return _mutants;
            }
        }

        public IEnumerable<TestTreeNode> TestsToRun { get; set; }

        private string PropName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            return memberExpression.Member.Name;
        }

        public void ViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropName(() => _unitTestsVm.SelectedMutant))
            {
                RefreshTestList(_unitTestsVm.SelectedMutant.Assemblies);
            }
            else if (e.PropertyName == PropName(() => _unitTestsVm.TestCurrentSolution))
            {
                ChangeModeToCurrentSolution(_unitTestsVm.TestCurrentSolution);
            }
            else if (e.PropertyName == PropName(() => _unitTestsVm.SelectedTestItem))
            {
                var method = _unitTestsVm.SelectedTestItem as TestNodeMethod;
                if (method != null && method.HasResults)
                {
                    _unitTestsVm.ResultText = method.Result.Message;
                }   
                else
                {
                    _unitTestsVm.ResultText = "";
                }
            }
        }

        private void ChangeModeToCurrentSolution(bool testCurrentSolution)
        {
            if (testCurrentSolution)
            {
                RefreshTestList(_visualStudioConnection.GetProjectPaths());
            }
            else
            {
                RefreshTestList(_unitTestsVm.SelectedMutant.Assemblies);
            }
        }

        public void Initialize()
        {
            //  _tl.R
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

        public void RunTests()
        {
            
            
            TestsToRun = _unitTestsVm.TestNamespaces;
            _tl.RunTests();
        }
        void Events_RunStarting(object sender, TestEventArgs args)
        {
            _execute.OnUIThread(()=>
            {
                _unitTestsVm.AreTestsRunning = true;
                _commandRunTests.RaiseCanExecuteChanged();

                foreach (var testTreeNode in _testMap.Values)
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
                var node =_testMap[args.Result.Test.TestName.UniqueName];

                



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
                    _messageBoxService.Show(e.ToString());
                }
            }
        }

        private void Events_RunFinished(object sender, TestEventArgs args)
        {
            try
            {
                _execute.OnUIThread(() =>
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
                    _messageBoxService.Show(e.ToString());
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
                    _messageBoxService.Show(e.ToString());
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
                    _messageBoxService.Show(e.ToString());
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
                _messageBoxService.Show(args.Exception.ToString());
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
                _messageBoxService.Show(args.Exception.ToString());
            }
        }
    }
}