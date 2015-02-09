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
    using UsefulTools.Switches;

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