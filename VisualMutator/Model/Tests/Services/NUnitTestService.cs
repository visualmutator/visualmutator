namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using log4net;
    using NUnit.Core;
    using TestsTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    #endregion

    public class NUnitTestService : ITestService
    {
        private readonly INUnitWrapper _nUnitWrapper;


        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IDisposable _subscription;
        private IDisposable _subscriptionRun;
        private bool _currentRunCancelled;


        public NUnitTestService(INUnitWrapper nUnitWrapper, IMessageService messageService)
        {
            _nUnitWrapper = nUnitWrapper;

        }

        public INUnitWrapper TestLoader
        {
            get
            {
                return _nUnitWrapper;
            }
        }


        public virtual TestsLoadContext LoadTests(IList<string> assemblies)
        {
            _currentRunCancelled = false;
            var context = new TestsLoadContext();
            ITest testRoot = _nUnitWrapper.LoadTests(assemblies);
            if (testRoot.Tests.S)
            BuildTestTree(testRoot, context);
            return context;
        }

        public static IList<T> ConvertToListOf<T>(IList iList)
        {
            IList<T> result = new List<T>();
            if (iList != null)
            {
                foreach (T value in iList)
                {
                    result.Add(value);
                }
            }

            return result;
        }
        public virtual Task RunTests(TestsLoadContext context)
        {

            var list = new List<TestNodeMethod>();

           

            var sw = new Stopwatch();
         //   sw.Start();
            Task<TestResult> runTests = TestLoader.RunTests();


            return runTests.ContinueWith(testResult =>
            {
                if(testResult.Exception != null)
                {
                    _log.Error(testResult.Exception);
                   // return null;
                }
                else
                {
                    _subscription = TestLoader.TestFinished.Subscribe(result =>
                    {
                        TestNodeMethod node = context.TestMap[result.Test.TestName.FullName];
                        node.State = result.IsSuccess ? TestNodeState.Success : TestNodeState.Failure;
                        node.Message = result.Message + "\n" + result.StackTrace;
                        list.Add(node);
                    }, () =>
                    {

                        
                    });
                    _subscription.Dispose();
               
                  //  sw.Stop();
                  //  mutantTestSession.RunTestsTimeRawMiliseconds = sw.ElapsedMilliseconds;
                   // return list;
                }
            });
        
        }

        public void UnloadTests()
        {
            
            _nUnitWrapper.UnloadProject();
        }

        private void BuildTestTree(ITest test, TestsLoadContext context)
        {
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
                        string testName = testMethod.TestName.Name;
                        if(!context.TestMap.ContainsKey(testName))
                        {
                            var nodeMethod = new TestNodeMethod(c, testName);
                            nodeMethod.TestId = new NUnitTestId(testMethod.TestName);
                            c.Children.Add(nodeMethod);
                            _log.Debug("Adding test: " + testName);
                            context.TestMap.Add(testName, nodeMethod);
                        }
                        else
                        {
                            _log.Debug("Already exists test: " + testName);
                            //TODO: handle he case where parametrized test method may be present duplicated.
                        }
                    }
                }
                if(c.Children.Any())
                {
                    context.ClassNodes.Add(c);
                }
            }
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

        public virtual void CreateTestFilter(ICollection<TestId> selectedTests)
        {
            _nUnitWrapper.CreateFilter(selectedTests.Cast<NUnitTestId>().Select(id =>id.TestName).ToList());
        }





    }
}