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
    using Infrastructure;
    using log4net;
    using NUnit.Core;
    using RunProcessAsTask;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    #endregion

    public class NUnitXmlTestService : NUnitTestService
    {
        private readonly INUnitExternal _nUnitExternal;
        private readonly CommonServices _svc;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string _assemblyPath;


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


        public override May<TestsLoadContext> LoadTests(string assemblyPath)
        {
            _assemblyPath = assemblyPath;
            May<TestsLoadContext> loadTests = base.LoadTests(assemblyPath);

            UnloadTests();
            return loadTests;
        }
        private string FindConsolePath()
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
        public override Task RunTests(TestsRunContext context)
        {

          //  var sw = new Stopwatch();
          //  sw.Start();

            string runPath = FindConsolePath();

            _log.Info("Running NUnit Console from: " + runPath);
            string assemblyPath = context.AssemblyPath;
            var tasks = new List<Task<List<MyTestResult>>>();
            string name = string.Format("muttest-{0}.xml", Path.GetFileName(assemblyPath));
            string outputFilePath = new FilePathAbsolute(assemblyPath).GetBrotherFileWithName(name).ToString();

            if(string.IsNullOrWhiteSpace(context.SelectedTests.TestsDescription))
            {
                context.TestResults = new MutantTestResults(new List<TmpTestNodeMethod>());
                return Task.FromResult(0);
            }
            return _nUnitExternal.RunTests(runPath, assemblyPath,
                outputFilePath, context.SelectedTests)

                .ContinueWith(testResult =>
                {
                    var list = new List<TmpTestNodeMethod>();
                    if (testResult.Exception != null)
                    {
                        _log.Error(testResult.Exception);
                        //todo: erorrs
                    }
                    else
                    {
                        //todo: check for empty lists
                        IList<MyTestResult> testResults = testResult.Result; 

                        foreach (var myTestResult in testResults)
                        {
                            TmpTestNodeMethod node = new TmpTestNodeMethod(myTestResult.Name);
                            node.State = myTestResult.Success ? TestNodeState.Success : TestNodeState.Failure;
                            node.Message = myTestResult.Message + "\n" + myTestResult.StackTrace;
                            list.Add(node);
                        }
                        context.TestResults = new MutantTestResults(list);
                    }
                    _log.Debug("Finished processing tests.");
                }).LogErrors();
        }

        


    }
}