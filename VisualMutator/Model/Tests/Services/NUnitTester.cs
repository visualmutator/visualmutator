namespace VisualMutator.Model.Tests.Services
{
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

    public class NUnitTester
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly NUnitResultsParser _parser;
        private readonly IProcesses _processes;
        private readonly CommonServices _svc;
        private readonly string _nUnitConsolePath;
        private readonly TestsRunContext _context;
        private CancellationTokenSource _cancellationTokenSource;

        public NUnitTester(
            NUnitResultsParser parser,
            IProcesses processes,
            CommonServices svc,
            string nUnitConsolePath,
            TestsRunContext context)
        {
            _parser = parser;
            _processes = processes;
            _svc = svc;
            _nUnitConsolePath = nUnitConsolePath;
            _context = context;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task RunTests()
        {

            //  var sw = new Stopwatch();
            //  sw.Start();
            string assemblyPath = _context.AssemblyPath;
            string name = string.Format("muttest-{0}.xml", Path.GetFileName(assemblyPath));
            string outputFilePath = new FilePathAbsolute(assemblyPath).GetBrotherFileWithName(name).ToString();

            if (string.IsNullOrWhiteSpace(_context.SelectedTests.TestsDescription))
            {
                _context.TestResults = new MutantTestResults();
                return Task.FromResult(0);
            }
            return RunTests(_nUnitConsolePath, assemblyPath,
                outputFilePath, _context.SelectedTests)
                .ContinueWith(t =>
                {
                    _context.TestResults = t.Result;
                });
        }

        public void CancelRun()
        {
            
            _cancellationTokenSource.Cancel();
            _log.Debug("Requested cancellation for testing for " + _cancellationTokenSource.GetHashCode());

        }

        public Task<MutantTestResults> RunTests(string nunitConsolePath,
            string inputFile, string outputFile,
            SelectedTests selectedTests)
        {
            _log.Debug("Running tests on: " + inputFile);
            Task<ProcessResults> results = RunNUnitConsole(nunitConsolePath, inputFile, outputFile, selectedTests);

            return results.ContinueWith(testResult =>
            {
                if (testResult.Exception != null)
                {
                    _log.Error(testResult.Exception);
                    return new MutantTestResults();
                }
                else if (testResult.IsCanceled)
                {
                    _log.Error("Test run cancelled.");
                    return new MutantTestResults(cancelled: true);
                }
                else if (!_svc.FileSystem.File.Exists(outputFile))
                {
                    _log.Error("Test results in file: " + outputFile + " not found.");
                    return new MutantTestResults();
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
            }).LogErrors();
        }

        public Task<ProcessResults> RunNUnitConsole(string nunitConsolePath,
            string inputFile, string outputFile,
            SelectedTests selectedTests)
        {
            var listpath = new FilePathAbsolute(inputFile)
                .GetBrotherFileWithName(
                Path.GetFileNameWithoutExtension(inputFile) + "-Runlist.txt").Path;
            //   var listpath = Path.Combine(Path.GetTempPath(), "visualMutator-Runlist.txt");
            using (var file = File.CreateText(listpath))
            {
                foreach (var str in selectedTests.TestsDescription.Split(' '))
                {
                    file.WriteLine(str.Trim());
                }
            }
            // string testToRun = (selectedTests.Count == 0 ? "": " -run " + 
            //  string.Join(",",selectedTests.Cast<NUnitTestId>().Select(id => id.TestName.FullName)));
            string testToRun = " /runlist:" + listpath.InQuotes() + " ";
            string arg = inputFile.InQuotes()
                         + testToRun
                         + " /xml \"" + outputFile + "\" /nologo -trace=Verbose";
            //  + " -framework net-4.0";

            _log.Info("Running " + nunitConsolePath + " with args: " + arg);
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