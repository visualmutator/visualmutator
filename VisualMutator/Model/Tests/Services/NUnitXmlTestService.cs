namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using CoverageFinder;
    using Exceptions;
    using Infrastructure;
    using log4net;
    using NUnit.Core;
    using RunProcessAsTask;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    #endregion

    public class NUnitXmlTestService : ITestsService
    {
        private readonly IFactory<TestsRunContext> _testsRunContextFactory;
        private readonly ISettingsManager _settingsManager;
        private readonly CommonServices _svc;

        private const string FrameworkName = "NUnit";

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _nunitConsolePath;
        private readonly INUnitWrapper _nUnitWrapper;

        public string NunitConsolePath
        {
            get { return _nunitConsolePath; }
        }
        
        public NUnitXmlTestService(
            IFactory<TestsRunContext> testsRunContextFactory,
            ISettingsManager settingsManager,
            INUnitWrapper nUnitWrapper,
            CommonServices svc)
          
        {
            _testsRunContextFactory = testsRunContextFactory;
            _settingsManager = settingsManager;
            _nUnitWrapper = nUnitWrapper;
            _svc = svc;

            _nunitConsolePath = FindConsolePath();
            _log.Info("Set NUnit Console path: " + _nunitConsolePath);
        }

        public void Cancel()
        {
            
        }

        public TestsRunContext CreateRunContext(TestsLoadContext loadContext, string mutatedPath)
        {
            return _testsRunContextFactory.CreateWithParams(loadContext, mutatedPath);
        }

        private string FindConsolePath()
        {
            var nUnitDirPath = _settingsManager["NUnitConsoleDirPath"];
            var nUnitConsolePath = Path.Combine(nUnitDirPath, "nunit-console-x86.exe");
            
            if (!_svc.FileSystem.File.Exists(nUnitConsolePath))
            {
                throw new FileNotFoundException(nUnitConsolePath + " file was found.");
            }
            return nUnitConsolePath;
        }


        public string FrameWorkName { get { return FrameworkName; } }

        public virtual May<TestsLoadContext> LoadTests(string assemblyPath)
        {
            
            ITest testRoot = _nUnitWrapper.LoadTests(assemblyPath.InList());
            int testCount = testRoot.TestsEx().SelectMany(n => n.TestsEx()).Count();
            if (testCount == 0)
            {
                return May.NoValue;
            }
            var classNodes = BuildTestTree(testRoot);
            var context = new TestsLoadContext(FrameworkName, classNodes.ToList());
            UnloadTests();
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


        public void UnloadTests()
        {
            // _nUnitWrapper.UnloadProject();
        }

        private IEnumerable<TestNodeClass> BuildTestTree(ITest test)
        {
            IEnumerable<ITest> classes = GetTestClasses(test).ToList();

            foreach (ITest testClass in classes.Where(c => c.Tests != null && c.Tests.Count != 0))
            {

                var c = new TestNodeClass(testClass.TestName.Name)
                {
                    Namespace = testClass.Parent.TestName.FullName,
                    //  FullName = testClass.TestName.FullName,

                };

                foreach (ITest testMethod in testClass.Tests.Cast<ITest>())
                {
                    if (_nUnitWrapper.NameFilter == null || _nUnitWrapper.NameFilter.Match(testMethod))
                    {
                        string testName = testMethod.TestName.FullName;
                        //if(!context.TestMap.ContainsKey(testName))
                        //  {
                        var nodeMethod = new TestNodeMethod(c, testName)
                        {
                            TestId = new NUnitTestId(testMethod.TestName),
                            Identifier = CreateIdentifier(testMethod),
                        };
                        c.Children.Add(nodeMethod);
                        _log.Debug("Adding test: " + testName);
                        // context.TestMap.Add(testName, nodeMethod);
                        // }
                        //  else
                        //  {
                        //       _log.Debug("Already exists test: " + testName);
                        //       //TODO: handle he case where parametrized test method may be present duplicated.
                        //   }
                    }
                }
                if (c.Children.Any())
                {
                    yield return c;
                  
                }
            }
        }

        private MethodIdentifier CreateIdentifier(ITest testMethod)
        {
            return new MethodIdentifier(testMethod.TestName.FullName + "()");
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




    }
}