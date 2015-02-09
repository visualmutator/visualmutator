namespace VisualMutator.Model.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using log4net;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;

    public class MsTestResultsParser
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        public Dictionary<string, TmpTestNodeMethod> ProcessResultFile(string fileName)
        {
            _log.Debug("Processing result file: " + fileName);
            var s = new XmlReaderSettings {CheckCharacters = false};
            var resultDictionary = new Dictionary<string, TmpTestNodeMethod>();
            using(XmlReader reader = XmlReader.Create(fileName, s))
            {
                var doc = XDocument.Load(reader);

                foreach (XElement testResult in doc.Root.DescendantsAnyNs("UnitTestResult"))
                {
                    string value = testResult.Attribute("testId").Value;
                    var unitTest = doc.Root.DescendantsAnyNs("UnitTest")
                        .Single(n => n.Attribute("id").Value == value);
                    var testMethod = unitTest.ElementAnyNs("TestMethod");

                    string methodName = testMethod.Attribute("name").Value;
                    string longClassName = testMethod.Attribute("className").Value;

                    string fullClassName = longClassName.Substring(0, longClassName.IndexOf(","));

                    var node = new TmpTestNodeMethod(fullClassName + "." + methodName);


                    node.State = TranslateTestResultStatus(testResult.Attribute("outcome").Value);

                    if (node.State == TestNodeState.Failure)
                    {
                        var errorInfo = testResult.DescendantsAnyNs("ErrorInfo").Single();
                        node.Message = errorInfo.ElementAnyNs("Message").Value;
                    }

                    resultDictionary.Add(node.Name, node);

                }
            }
           
            return resultDictionary;
        }
        
        private TestNodeState TranslateTestResultStatus(string status)
        {
            switch (status)
            {
                case "Passed":
                    return TestNodeState.Success;
                case "Failed":
                    return TestNodeState.Failure;
                case "Inconclusive":
                    return TestNodeState.Inconclusive;
                default:
                    throw new ArgumentException("status");
            }
        }


    }
}