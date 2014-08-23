namespace VisualMutator.Model.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using log4net;
    using RunProcessAsTask;
    using TestsTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    public class NUnitTestsRunContext : ITestsRunContext
    {
        public MutantTestResults TestResults
        {
            get { return _testResults; }
        }


        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly OptionsModel _options;
        private readonly NUnitResultsParser _parser;
        private readonly IProcesses _processes;
        private readonly CommonServices _svc;
        private readonly string _assemblyPath;
        private readonly string _nUnitConsolePath;
        private readonly TestsSelector _testsSelector;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private MutantTestResults _testResults;

        public NUnitTestsRunContext(
            OptionsModel options,
            IProcesses processes,
            CommonServices svc, 
            NUnitResultsParser parser,
            //----------
            string nUnitConsolePath,
            string assemblyPath,
            TestsSelector testsSelector)
        {
            _options = options;
            _parser = parser;
            _processes = processes;
            _svc = svc;
            _assemblyPath = assemblyPath;
            _nUnitConsolePath = nUnitConsolePath;
            _testsSelector = testsSelector;
            _cancellationTokenSource = new CancellationTokenSource();

           // var testsSelector = new TestsSelector();
          //  _selectedTests = testsSelector.GetIncludedTests(loadContext.Namespaces);
          //  _log.Debug("Created tests to run: " + _selectedTests.TestsDescription);
        }

        public async Task<MutantTestResults> RunTests()
        {
            string name = string.Format("muttest-{0}.xml", Path.GetFileName(_assemblyPath));
            string outputFilePath = new FilePathAbsolute(_assemblyPath).GetBrotherFileWithName(name).ToString();

            if (_testsSelector.IsEmpty)
            {
                _testResults = new MutantTestResults();
                return TestResults;
            }
            _testResults = await RunTests(_assemblyPath,
                outputFilePath);
            return TestResults;
        }

        public void CancelRun()
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch (Exception e)
            {
                _log.Warn("Exception while cancelling: "+e);
            }
            _log.Debug("Requested cancellation for testing for " + _cancellationTokenSource.GetHashCode());
        }

        public async Task<MutantTestResults> RunTests(
            string inputFile, string outputFile)
        {
            _log.Debug("Running tests on: " + inputFile);

            try
            {
                ProcessResults results = await RunNUnitConsole(_nUnitConsolePath, inputFile, outputFile);
                if (!_svc.FileSystem.File.Exists(outputFile))
                {

                    throw new Exception("Test results in file: " + outputFile + " not found. Output: " +
                        results.StandardOutput.Aggregate((a, b) => a + "\n" + b));
                }
                else
                {
                    Dictionary<string, MyTestResult> tresults = _parser.ProcessResultFile(outputFile);

                    IList<MyTestResult> testResults = tresults.Values.ToList();
                    var list = new List<TmpTestNodeMethod>();
                    foreach (var myTestResult in testResults)
                    {
                        TmpTestNodeMethod node = new TmpTestNodeMethod(myTestResult.Name);
                        node.State = myTestResult.Success ? TestNodeState.Success : TestNodeState.Failure;
                        node.Message = myTestResult.Message + "\n" + myTestResult.StackTrace;
                        list.Add(node);
                    }

                    return new MutantTestResults(list);
                }
            }
            catch (OperationCanceledException)
            {
                _log.Error("Test run cancelled.");
                return new MutantTestResults(cancelled: true);
            }


        }

        public Task<ProcessResults> RunNUnitConsole(string nunitConsolePath,
            string inputFile, string outputFile)
        {
            var listpath = new FilePathAbsolute(inputFile)
                .GetBrotherFileWithName(
                Path.GetFileNameWithoutExtension(inputFile) + "-Runlist.txt").Path;

            string testToRun = "";
            if(!_testsSelector.AllowAll)
            {
                using (var file = File.CreateText(listpath))
                {
                    foreach (var str in _testsSelector.MinimalSelectionList)
                    {
                        file.WriteLine(str.Trim());
                    }
                }
                testToRun = " /runlist:" + listpath.InQuotes() + " ";
            }

            string arg = inputFile.InQuotes()
                         + testToRun
                         + " /xml \"" + outputFile + "\" /nologo -trace=Verbose /noshadow /nothread";

            if (_options.ParsedParams.NUnitNetVersion.Length != 0)
            {
                arg += (" /framework:" + _options.OtherParams);
            }

            _log.Info("Running \"" + nunitConsolePath+"\" " + arg);
            var startInfo = new ProcessStartInfo
            {
                Arguments = arg,
                CreateNoWindow = true,
                ErrorDialog = true,
                RedirectStandardOutput = false,
                FileName = nunitConsolePath,
                UseShellExecute = false,
            };
            return _processes.RunAsync(startInfo, _cancellationTokenSource);
        }




    }
}