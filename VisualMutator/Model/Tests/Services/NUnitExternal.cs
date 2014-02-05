namespace VisualMutator.Model.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using log4net;
    using NUnit.Core;
    using RunProcessAsTask;
    using TestsTree;
    using UsefulTools.ExtensionMethods;

    public class NUnitExternal
    {
        private readonly string _nunitConsolePath;
        Dictionary<string, MyTestResult> resultDictionary;

        public NUnitExternal(
            string nunitConsolePath)
            : base()
        {
            _nunitConsolePath = nunitConsolePath;
            resultDictionary = new Dictionary<string, MyTestResult>();
        }

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        


        public Task<ProcessResults> RunNUnitConsole(IList<string> inputFile, string outputFile, 
            ICollection<TestId> selectedTests)
        {
            string testToRun = (selectedTests.Count == 0 ? "": " -run " + 
                selectedTests.Cast<NUnitTestId>().Select(id => id.TestName.FullName)
                             .Aggregate((a, b) => a + ","+b));
            string arg = inputFile.Aggregate((a, b) => a.InQuotes() + " " + b.InQuotes())
                + testToRun
                + " /xml \"" + outputFile + "\" /nologo /nothread";

            _log.Info("Running " + _nunitConsolePath + " with args: " + arg);
            var startInfo = new ProcessStartInfo
            {
                Arguments = arg,
                CreateNoWindow = true,
                ErrorDialog = true,
                RedirectStandardOutput = false,
                FileName = _nunitConsolePath,
                UseShellExecute = false,
            };

            return ProcessEx.RunAsync(startInfo);
        }
        
        public Dictionary<string, MyTestResult> ProcessResultFile( string fileName)
        {
            XmlReaderSettings s = new XmlReaderSettings();
            s.CheckCharacters = false;

            XmlReader reader = XmlReader.Create(fileName, s);

            var nsStack = new Stack<String>();
            bool isParametrizedTest = false;
            TestNodeClass currentClass = null;
            try
            {
                while (reader.Read())
                {
                    if (reader.Name == "test-suite")
                    {
                        if (reader.GetAttribute("type") == "Namespace" && reader.NodeType == XmlNodeType.Element)
                        {
                            nsStack.Push(reader.GetAttribute("name"));
                        }
                        if (reader.GetAttribute("type") == "Namespace" && reader.NodeType == XmlNodeType.EndElement)
                        {
                            nsStack.Pop();
                        }

                        if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            isParametrizedTest = false;
                            currentClass = null;
                        }
                        else if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.GetAttribute("type") == "ParameterizedTest")
                            {
                                isParametrizedTest = true;
                            }
                            if (reader.GetAttribute("type") == "TestFixture")
                            {
                                isParametrizedTest = false;
                                currentClass = new TestNodeClass(reader.GetAttribute("name"));
                                currentClass.Namespace = nsStack.Aggregate((a, b) => a + "." + b);
                            }
                        }
                    }
                    else if (reader.Name == "test-case" && reader.NodeType == XmlNodeType.Element)
                    {
                        var node = (XElement)XNode.ReadFrom(reader);

                        GetValue(node, isParametrizedTest);
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
        
        private void GetValue( XElement test, bool isParametrizedTest)
        {
            
            try
            {
                String testName = test.Attribute("name").Value;
                if (isParametrizedTest)
                {
                    int paranIdx = testName.IndexOf('(');
                    if (paranIdx > 0)
                        testName = testName.Substring(0, paranIdx);
                }
                String testRes = "False";
               

                var result = new MyTestResult();
                if (test.Attribute("success") != null)
                {
                    result.Success = test.Attribute("success").Value == "True";
                }
                if (!result.Success)
                {
                    result.Message = test.Descendants(XName.Get("message", "")).Single().Value;
                    result.StackTrace = test.Descendants(XName.Get("stack-trace", "")).Single().Value;
                }
                
                resultDictionary.Add(test.Attribute("name").Value, result);
              
            }
            catch (Exception e)
            {
                _log.Error("Log file parsing error", e);
            }
        }
     
    }
}
