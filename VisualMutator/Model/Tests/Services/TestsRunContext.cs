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

    public class TestsRunContext
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
        private readonly SelectedTests _selectedTests;
        private readonly string _assemblyPath;
        private readonly string _nUnitConsolePath;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private MutantTestResults _testResults;

        public TestsRunContext(
            OptionsModel options,
            NUnitResultsParser parser,
            IProcesses processes,
            CommonServices svc,
            NUnitXmlTestService nunitXmlTestService,
            //----------
            SelectedTests selectedTests,
            string assemblyPath)
        {
            _options = options;
            _parser = parser;
            _processes = processes;
            _svc = svc;
            _selectedTests = selectedTests;
            _assemblyPath = assemblyPath;
            _nUnitConsolePath = nunitXmlTestService.NunitConsolePath;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task<MutantTestResults> RunTests()
        {
            string name = string.Format("muttest-{0}.xml", Path.GetFileName(_assemblyPath));
            string outputFilePath = new FilePathAbsolute(_assemblyPath).GetBrotherFileWithName(name).ToString();

            if (string.IsNullOrWhiteSpace(_selectedTests.TestsDescription)) //TODO: what is this?
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
                ProcessResults results = await RunNUnitConsole(_nUnitConsolePath, inputFile, outputFile, _selectedTests);
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
            string inputFile, string outputFile,
            SelectedTests selectedTests)
        {
            var listpath = new FilePathAbsolute(inputFile)
                .GetBrotherFileWithName(
                Path.GetFileNameWithoutExtension(inputFile) + "-Runlist.txt").Path;
            using (var file = File.CreateText(listpath))
            {
                foreach (var str in selectedTests.TestsDescription.Split(' '))
                {
                    file.WriteLine(str.Trim());
                }
            }
            string testToRun = " /runlist:" + listpath.InQuotes() + " ";
            string arg = inputFile.InQuotes()
                         + testToRun
                         + " /xml \"" + outputFile + "\" /nologo -trace=Verbose /noshadow /nothread";

            if (_options.ParsedParams.NUnitNetVersion.Length != 0)
            {
                arg += (" /framework:" + _options.OtherParams);
            }

            _log.Info("Running " + nunitConsolePath + arg);
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