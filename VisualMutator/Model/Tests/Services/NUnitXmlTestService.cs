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
    using Exceptions;
    using log4net;
    using NUnit.Core;
    using RunProcessAsTask;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;

    #endregion

    public class NUnitXmlTestService : NUnitTestService
    {
        private readonly INUnitExternal _nUnitExternal;
        private readonly CommonServices _svc;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IList<string> _assemblies;
        private ICollection<TestId> _selectedTests;

        public IList<string> Assemblies
        {
            get { return _assemblies; }
            set { _assemblies = value; }
        }
        
        public string NUnitConsoleAltPath { get; set; }
        public string NUnitConsolePath { get; set; }

        public NUnitXmlTestService(
            INUnitWrapper nUnitWrapper, 
            INUnitExternal nUnitExternal, 
            CommonServices svc)
            : base(nUnitWrapper, svc.Logging)
        {
            _nUnitExternal = nUnitExternal;
            _svc = svc;

            var localPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            NUnitConsoleAltPath = Path.Combine(Path.GetDirectoryName(localPath), "nunit-console.exe");
            NUnitConsolePath = Path.Combine(Path.GetDirectoryName(localPath), "nunit-console-x86.exe");

        }


        public override May<TestsLoadContext> LoadTests(IList<string> assemblies)
        {
            _assemblies = assemblies;
            May<TestsLoadContext> loadTests = base.LoadTests(assemblies);

            UnloadTests();
            return loadTests;
        }
        private string findConsolePath()
        {
            string runPath;
            if (_svc.FileSystem.File.Exists(NUnitConsolePath))
            {
                runPath = NUnitConsolePath;
            }
            else if (_svc.FileSystem.File.Exists(NUnitConsoleAltPath))
            {
                runPath = NUnitConsoleAltPath;
            }
            else
            {
                throw new FileNotFoundException(NUnitConsolePath + " nor " + NUnitConsoleAltPath + " file was found.");
            }
            return runPath;
        }
        public override Task RunTests(TestsLoadContext context)
        {

          //  var sw = new Stopwatch();
          //  sw.Start();

            string runPath = findConsolePath();

            _log.Info("Running NUnit Console from: " + runPath);

            var tasks = new List<Task<List<MyTestResult>>>();
            int ordinal = 0;
            foreach (var assembly in _assemblies)
            {
                string name = string.Format("muttest{0}-{1}.xml", ordinal, Path.GetFileName(assembly));
                Task<List<MyTestResult>> task = _nUnitExternal.RunTests(runPath, assembly.InList(),
                    name, _selectedTests);
                tasks.Add(task);
                ordinal++;
            }

            return Task.WhenAll(tasks)
                .ContinueWith( testResult =>
                {
                    if(testResult.Exception != null)
                    {
                        _log.Error(testResult.Exception);
                        //todo: erorrs
                        return new List<TestNodeMethod>();
                    }
                    else
                    {
                        //todo: check for empty lists
                        IList<MyTestResult> testResults = testResult.Result.Flatten().ToList();

                        foreach (var myTestResult in testResults)
                        {
                            TestNodeMethod testNodeMethod = context.TestMap[myTestResult.Name];
                            testNodeMethod.Message = myTestResult.Message + "\n" + myTestResult.StackTrace;
                            testNodeMethod.State = myTestResult.Success
                                ? TestNodeState.Success
                                : TestNodeState.Failure;
                        }
                       // sw.Stop();
                       // mutantTestSession.RunTestsTimeRawMiliseconds = sw.ElapsedMilliseconds;
                        return new List<TestNodeMethod>();
                    }
            });
        }

        public override void CreateTestFilter(ICollection<TestId> selectedTests)
        {
            _selectedTests = selectedTests;
            base.CreateTestFilter(selectedTests);
        }


        


    }
}