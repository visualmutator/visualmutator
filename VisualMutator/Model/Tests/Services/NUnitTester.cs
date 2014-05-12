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

    public class NUnitTester
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly NUnitResultsParser _parser;
        private readonly IProcesses _processes;
        private readonly CommonServices _svc;
        private readonly string _nUnitConsolePath;
        private readonly TestsRunContext _context;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private OptionsModel _options;

        public NUnitTester(
            OptionsManager optionsManager,
            NUnitResultsParser parser,
            IProcesses processes,
            CommonServices svc,
            string nUnitConsolePath,
            TestsRunContext context)
        {
            _options = optionsManager.ReadOptions();
            _parser = parser;
            _processes = processes;
            _svc = svc;
            _nUnitConsolePath = nUnitConsolePath;
            _context = context;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task<MutantTestResults> RunTests()
        {
            string assemblyPath = _context.AssemblyPath;
            string name = string.Format("muttest-{0}.xml", Path.GetFileName(assemblyPath));
            string outputFilePath = new FilePathAbsolute(assemblyPath).GetBrotherFileWithName(name).ToString();

            if (string.IsNullOrWhiteSpace(_context.SelectedTests.TestsDescription))
            {
                _context.TestResults = new MutantTestResults();
                return _context.TestResults;
            }
            _context.TestResults = await RunTests(_nUnitConsolePath, assemblyPath,
                outputFilePath, _context.SelectedTests);
            return _context.TestResults;
        }

        public void CancelRun()
        {
            _cancellationTokenSource.Cancel();
            _log.Debug("Requested cancellation for testing for " + _cancellationTokenSource.GetHashCode());
        }

        public async Task<MutantTestResults> RunTests(string nunitConsolePath,
            string inputFile, string outputFile,
            SelectedTests selectedTests)
        {
            _log.Debug("Running tests on: " + inputFile);

            try
            {
                ProcessResults results = await RunNUnitConsole(nunitConsolePath, inputFile, outputFile, selectedTests);
                if (!_svc.FileSystem.File.Exists(outputFile))
                {
                    
                    throw new Exception("Test results in file: " + outputFile + " not found. Output: "+
                        results.StandardOutput.Aggregate((a,b)=>a+"\n"+b));
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

            if(_options.ForceNUnitDotNedVer.Length != 0)
            {
                arg += (" /framework:" + _options.ForceNUnitDotNedVer);
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