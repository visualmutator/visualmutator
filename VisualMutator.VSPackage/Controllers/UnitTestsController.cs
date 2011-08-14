namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    #region Usings

    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Collections.Generic;
   
    using System.Diagnostics;
    using System.IO;
    using System.Linq.Expressions;
    using System.Text;
    using System.Waf.Applications;
    using System.Windows;

    using Mono.Collections.Generic;

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
        private readonly UnitTestsViewModel _unitTestsVm;

        private readonly IVisualStudioConnection _visualStudioConnection;

        private ObservableCollection<MutationSession> _mutants;

        private readonly TestLoader _tl;

        private Dictionary<string, TestTreeNode> _testMap; 


        public UnitTestsController(
            UnitTestsViewModel unitTestsVm,
            IVisualStudioConnection visualStudioConnection
            )
        {
            _unitTestsVm = unitTestsVm;
            _visualStudioConnection = visualStudioConnection;

            _unitTestsVm.CommandRefresh = new DelegateCommand(RunTests);
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
            _tl.Events.ProjectLoadFailed += new TestEventHandler(Events_ProjectLoadFailed);
            _tl.Events.ProjectLoaded += new TestEventHandler(Events_ProjectLoaded);
            _tl.Events.TestLoadFailed += new TestEventHandler(Events_TestLoadFailed);
            _tl.Events.TestLoaded += new TestEventHandler(Events_TestLoaded);
            _tl.Events.TestFinished += new TestEventHandler(Events_TestFinished);
            _tl.Events.SuiteFinished += new TestEventHandler(Events_SuiteFinished);
            _tl.Events.RunFinished += new TestEventHandler(Events_RunFinished);
           // _tl.Events.
        }


       
        private string PropName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            return memberExpression.Member.Name;
        }

        public void ViewModelChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == PropName(() => _unitTestsVm.SelectedMutant) )
            {
                RefreshTestList(_unitTestsVm.SelectedMutant.Assemblies);
            }
            else if (e.PropertyName == PropName(() => _unitTestsVm.TestCurrentSolution) )
            {
                ChangeModeToCurrentSolution(_unitTestsVm.TestCurrentSolution);
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

        public void Initialize()
        {
          //  _tl.R
        }
        private TestFilter MakeNameFilter(ICollection<ITest> tests)
        {
            if (tests == null || tests.Count == 0)
                return TestFilter.Empty;

            NameFilter nameFilter = new NameFilter();
            foreach (ITest test in tests)
                nameFilter.Add(test.TestName);

            return nameFilter;
        }

        public IEnumerable<TestTreeNode> TestsToRun { get; set; }
     

        public void RunTests()
        {
            TestsToRun = _unitTestsVm.TestNamespaces;
            _tl.RunTests();
            
        }

        void Events_TestFinished(object sender, TestEventArgs args)
        {
          //  ICollection<ITest> classes = GetTestClasses(args.Test);
          //  var namespaces = classes.Select(x => x.Parent).Distinct();
            try
            {
                TestStatus status = args.Result.IsSuccess ? TestStatus.Success : TestStatus.Failure;
                _testMap[args.Result.Test.TestName.UniqueName].Status = status;

                using (var s = File.AppendText(@"C:\test.txt"))
                {
                    s.WriteLine("TestFinished   "+args.Result.Name+"  s?: "+args.Result.IsSuccess);
                }
            }
            catch (Exception e)
            {
                throw;
            }
          
        }



        void Events_RunFinished(object sender, TestEventArgs args)
        {
            try
            {
             
                using (var s = File.AppendText(@"C:\test.txt"))
                {
                    s.WriteLine("RunFinished   " + args.Result.Name + "  s?: " + args.Result.IsSuccess);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
           
        }

        void Events_SuiteFinished(object sender, TestEventArgs args)
        {
            try
            {
              

                using (var s = File.AppendText(@"C:\test.txt"))
                {
                    s.WriteLine("SuiteFinished   " + args.Result.Name + "  s?: " + args.Result.IsSuccess);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void RefreshTestList(IEnumerable<string> assemblies)
        {
         
            _tl.NewProject();
            foreach (var project in assemblies)
            {
                _tl.TestProject.ActiveConfig.Assemblies.Add(project);
            }
     
          
            _tl.LoadTest();

        }

        void Events_TestLoaded(object sender, TestEventArgs args)
        {
            try
            {
                _unitTestsVm.TestNamespaces.Clear();
                _testMap.Clear();
                ICollection<ITest> classes = GetTestClasses(args.Test);
                var namespaces = classes.Select(x => x.Parent).Distinct();
                foreach (var testNamespace in namespaces)
                {
                    var ns = new TestNodeNamespace()
                    {
                        Name = testNamespace.TestName.FullName,
                        Test = testNamespace,
                    };
                    ITest testNamespace1 = testNamespace;
                    foreach (var testClass in classes.Where(c => c.Tests != null 
                        && c.Tests.Count != 0 && c.Parent == testNamespace1))
                    {
                        var c = new TestNodeClass()
                        {
                            Name = testClass.TestName.Name,
                            Test = testClass,
                        };
                        
                        foreach (var testMethod in testClass.Tests.Cast<ITest>())
                        {
                            var m = new TestNodeMethod()
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
            }
            catch (Exception e)
            {
                
                
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
            if(test.TestType == "TestFixture")
            {
                collection.Add(test);
                return;
            }
            else if (test.Tests != null)
            {
                foreach (var t in test.Tests.Cast<ITest>())
                {
                    GetTestClassesInternal(collection, t);
                }
            }
        }
        void Print(string pref, ITest info, StringBuilder sb)
        {
            sb.AppendLine(pref+ info.TestName);
            sb.AppendLine(pref+ "IsSuite: "+ info.IsSuite);
            sb.AppendLine(pref + "TestType: " + info.TestType);
            sb.AppendLine(pref);
            if(info.Tests != null)
            foreach (var test in info.Tests.Cast<ITest>())
            {
                Print(pref+"   ", test, sb);
            }
        }
        void Events_TestLoadFailed(object sender, TestEventArgs args)
        {
           
           // throw new NotImplementedException();
        }

        void Events_ProjectLoaded(object sender, TestEventArgs args)
        {
            //throw new NotImplementedException();
        }

        void Events_ProjectLoadFailed(object sender, TestEventArgs args)
        {
           // throw new NotImplementedException();
        }



        

    }
}