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

    public class XUnitTestsRunContext : ITestsRunContext
    {
        public MutantTestResults TestResults
        {
            get { return _testResults; }
        }


        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly XUnitResultsParser _parser;
        private readonly IProcesses _processes;
        private readonly CommonServices _svc;
        private readonly string _assemblyPath;
        private readonly TestsSelector _testsSelector;
        private readonly string _nUnitConsolePath;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private MutantTestResults _testResults;

        public XUnitTestsRunContext(
            XUnitResultsParser parser,
            IProcesses processes,
            CommonServices svc,
            //----------
            string xUnitPath,
            string assemblyPath,
            TestsSelector testsSelector)
        {
            _parser = parser;
            _processes = processes;
            _svc = svc;
            _assemblyPath = assemblyPath;
            _testsSelector = testsSelector;
            _nUnitConsolePath = xUnitPath;// @"C:\PLIKI\DOWNLOAD\xunit-2.0-beta-3\src\xunit.console\bin\Debug\xunit.console.exe";
            _cancellationTokenSource = new CancellationTokenSource();

           // var testsSelector = new TestsSelector();
           // _selectedTests = testsSelector.GetIncludedTests(loadContext.Namespaces);
            //_log.Debug("Created tests to run: " + _selectedTests.TestsDescription);
        }

        public async Task<MutantTestResults> RunTests()
        {
            string name = string.Format("muttest-{0}.xml", Path.GetFileName(_assemblyPath));
            string outputFilePath = new FilePathAbsolute(_assemblyPath).GetBrotherFileWithName(name).ToString();
            if(File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }
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
                _log.Debug("Process finished.");
                if (!_svc.FileSystem.File.Exists(outputFile))
                {
                    string output = results.StandardOutput
                        .Concat(results.StandardError)
                        .Aggregate((a, b) => a + "\n" + b);
                    if (output.Contains("Process is terminated due to StackOverflowException."))
                    {
                        TmpTestNodeMethod node = new TmpTestNodeMethod("One of the tests.");
                        node.State = TestNodeState.Failure;
                        node.Message = "One of the tests threw StackOverflowException.";
                        _log.Info("XUnit: One of the tests threw StackOverflowException.");
                        return new MutantTestResults(new List<TmpTestNodeMethod> { node });
                    }
                    else
                    {
                        throw new Exception("Test results in file: " + outputFile + " not found. Output: " + output);
                    }
                }
                else
                {
                    Dictionary<string, TmpTestNodeMethod> tresults = _parser.ProcessResultFile(outputFile);

                    List<TmpTestNodeMethod> testResults = tresults.Values.ToList();
                    var count = testResults
                        .Select(t => t.State).GroupBy(t => t)
                        .ToDictionary(t => t.Key, t => t.Count());

                    _log.Info(string.Format("XUnit test results: Passed: {0}, Failed: {1}, Inconc: {2}",
                        count.GetOrDefault(TestNodeState.Success),
                        count.GetOrDefault(TestNodeState.Failure),
                        count.GetOrDefault(TestNodeState.Inconclusive)));
                    return new MutantTestResults(testResults);
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
            string testToRun = "";
            if (!_testsSelector.AllowAll)
            {
                testToRun = " -names " + string.Join(";", _testsSelector.MinimalSelectionList) + " ";
            }
            string arg = inputFile.InQuotes()  + testToRun
                         + " -xmlv1 " + outputFile.InQuotes() + " ";


            _log.Info("Running: " + nunitConsolePath.InQuotes() + " " + arg);
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