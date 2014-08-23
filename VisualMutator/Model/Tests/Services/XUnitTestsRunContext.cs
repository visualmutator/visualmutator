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
    using System.Xml;
    using System.Xml.Linq;
    using Infrastructure;
    using log4net;
    using RunProcessAsTask;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using UsefulTools.Switches;
    using Switch = UsefulTools.Switches.Switch;

    public static class DictExt
    {
        public static V GetOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defaultValue = default(V))
        {
            V val;
            return dict.TryGetValue(key, out val) ? val : defaultValue;
        }
    }

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

    public class XUnitResultsParser
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        public Dictionary<string, TmpTestNodeMethod> ProcessResultFile(string fileName)
        {
            _log.Debug("Processing result file: " + fileName);
            var s = new XmlReaderSettings {CheckCharacters = false};
            var resultDictionary = new Dictionary<string, TmpTestNodeMethod>();
            XmlReader reader = XmlReader.Create(fileName, s);

            bool isParametrizedTest = false;
            try
            {
                while (reader.Read())
                {
                    if (reader.Name == "test" && reader.NodeType == XmlNodeType.Element)
                    {
                        var node = (XElement)XNode.ReadFrom(reader);

                        GetValue(resultDictionary, node);
                    }

                }
            }
            catch (Exception e)
            {
                _log.Error("Log file parsing error", e);
                throw;
            }

            reader.Close();
            return resultDictionary;
        }

        private void GetValue(Dictionary<string, TmpTestNodeMethod> resultDictionary, XElement test)
        {
            try
            {
                var result = new TmpTestNodeMethod(test.Attribute("name").Value);
                result.State = Switch.Into<TestNodeState>()
                    .From(test.Attribute("result").Value)
                    .Case("Pass", TestNodeState.Success)
                    .Case("Skip", TestNodeState.Inactive)
                    .Case("Fail", TestNodeState.Failure)
                    .Default(TestNodeState.Inconclusive);

                if (result.State != TestNodeState.Success)
                {
                    result.Message = test.Descendants(XName.Get("message", ""))
                        .Select(d => d.Value).MaySingle().Else("");
                    result.StackTrace = 
                        test.Descendants(XName.Get("stack-trace", "")).Select(d => d.Value).MaySingle().Else("");

                    result.Message = result.Message + "\n" + result.StackTrace;
                }

                resultDictionary.Add(result.Name, result);

            }
            catch (Exception e)
            {
                _log.Error("Log file parsing error", e);
            }
        }

    
    }
}