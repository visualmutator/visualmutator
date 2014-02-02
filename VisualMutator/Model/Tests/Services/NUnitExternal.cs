namespace VisualMutator.Model.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using log4net;
    using TestsTree;

    public class NUnitExternal
    {
        public NUnitExternal()
            : base()
        {
        }
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //public TestRunnerNUnit(Visualisation.TestsFrame testsFrame)
        //    : base(testsFrame)
        //{
        //    testingTool = TestingTool.NUNIT;
        //    ToolName = "NUnit";
        //}

        //public override bool RunExternalTestToolForFile(string fileName)
        //{
        //    string name = new FileInfo(fileName).Name;
        //    name = name.Remove(name.LastIndexOf('.'));
        //    string arg = fileName + " /xml " + outputDirectory + "\\" + name + "-result.xml /nologo";
        //    Process.Start(MutationManager.MutationManager.Me.MutationToolPath + "\\nunit-console", arg);
        //    return true;

        //}

        public bool RunExternalTestToolForSolution(string inputFile, string outputFile)
        {
            string arg = "\"" + inputFile + "\" /xml \"" + outputFile + "\" /nologo";


            var startInfo = new ProcessStartInfo
            {
                Arguments = arg,
                CreateNoWindow = true,
                ErrorDialog = true,
                RedirectStandardOutput = false,
                FileName = "nunit-console",
                UseShellExecute = false
            };

            Process proc = Process.Start(startInfo);
            bool res = proc.WaitForExit(1000 * 60);

            return (res && proc.ExitCode >= 0);
        }
        
        public bool ProcessResultFile(string mutName, string fileName, out Dictionary<string, bool> resultDictionary)
        {
            mutantsWithError = new List<string>();

            resultDictionary = new Dictionary<string, bool>();

            XmlReaderSettings s = new XmlReaderSettings();
            s.CheckCharacters = false;

            XmlReader reader = XmlReader.Create(fileName, s);

            // Sprawdzamy parsując plik z wynikami czy dany test nie jest parametryczny
            // Jeżeli jest, to z jego nazwy odrzucamy parametry (są one zesze na kończu nazwy w nawiasach),
            // bo często się zdarza, że są to wartości losowe i nie ma spójności w nazwach testów uryginalnych i mutantów
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

                        GetValue(node, currentClass, isParametrizedTest);
                    }
                
                }
            }
            catch (Exception e)
            {
                _log.Error("Log file parsing error", e);
                throw;
            }
            
            reader.Close();

         
        }
        
        private static void GetValue(string mutName, XElement test, bool isParametrizedTest)
        {
            Dictionary<string, bool> resultDictionary;
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
                if (test.Attribute("success") != null)
                {
                    testRes = test.Attribute("success").Value;
                }

                var methodNode = new TestNodeMethod(c, testMethod.TestName.Name);
                methodNode.TestId = new NUnitTestId(testMethod.TestName);
                c.Children.Add(methodNode);
                mutantTestSession.TestMap.Add(testMethod.TestName.FullName, m);

                if (!resultDictionary.ContainsKey(testName))
                {
                    if (testRes == "True")
                        resultDictionary.Add(testName, true);
                    else
                        resultDictionary.Add(testName, false);
                    if (testRes == "Error")
                        mutantsWithError.Add(mutName);
                }
                else
                {
                    int ii = 1;
                    while (resultDictionary.ContainsKey(testName + "_" + ii))
                    {
                        ii++;
                    }
                    if (testRes == "True")
                        resultDictionary.Add(testName + "_" + ii, true);
                    else
                        resultDictionary.Add(testName + "_" + ii, false);
                }
            }
            catch (Exception e)
            {
                Utilities.LogException(e);
                String testName = test.Attribute("testName").Value;
                int ii = 1;
                while (resultDictionary.ContainsKey(testName + "_" + ii))
                {
                    ii++;
                }
                resultDictionary.Add(testName + "_" + ii, false);
            }
        }
        /*
        public override void ProcessResultFileForTestsTimeSpan(string fileName, out Dictionary<string, TimeSpan> resultDictionary)
        {
            resultDictionary = new Dictionary<string, TimeSpan>();
            XmlReaderSettings s = new XmlReaderSettings();
            s.CheckCharacters = false;

            XmlReader reader = XmlReader.Create(fileName, s);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "test-case")
                        {
                            XElement tmpEle = XElement.ReadFrom(reader) as XElement;
                            if (tmpEle == null)
                                break;

                            try
                            {
                                String testName = tmpEle.Attribute("name").Value;
                                TimeSpan ts = new TimeSpan(0);
                                if (tmpEle.Attribute("success") != null)
                                    ts = TimeSpan.Parse("00.00:00:" + tmpEle.Attribute("time").Value);
                                if (!resultDictionary.ContainsKey(testName))
                                {

                                    resultDictionary.Add(testName, ts);
                                }
                                else
                                {
                                    int ii = 1;
                                    while (resultDictionary.ContainsKey(testName + "_" + ii))
                                    {
                                        ii++;
                                    }
                                    resultDictionary.Add(testName + "_" + ii, ts);
                                }



                            }
                            catch (Exception e)
                            {
                                Utilities.LogException(e);
                                String testName = tmpEle.Attribute("testName").Value;
                                int ii = 1;
                                while (resultDictionary.ContainsKey(testName + "_" + ii))
                                {
                                    ii++;
                                }
                                resultDictionary.Add(testName + "_" + ii, TimeSpan.Zero);

                            }
                        }
                        break;
                }
            }
            reader.Close();
        }

        public override bool CheckIfPathIsCorrect()
        {
            string filename = MutationManager.MutationManager.Me.MutationToolPath + "\\nunit-console.exe";
            FileInfo fi = new FileInfo(filename);

            return fi.Exists;
        }
        */
        //public override bool XmlTestCompare(string path1, string path2, out int passed, out int failed, out int testnum, out ArrayList testsList)
        //{
        //    passed = 0;
        //    failed = 0;
        //    testnum = 0;
        //    testsList = new ArrayList();

        //    XmlTextReader r1 = new XmlTextReader(new StreamReader(path1));
        //    XmlTextReader r2 = new XmlTextReader(new StreamReader(path2));
        //    bool result = true;
        //    try
        //    {

        //        //while (r1.Read() && r2.Read())
        //        while (r1.ReadToFollowing("test-case") && r2.ReadToFollowing("test-case"))
        //        {
        //            if (r1.Name != r2.Name) { result = false; continue; }
        //            if (r1.Name == "test-results" || r1.Name == "environment" || r1.Name == "culture-info" || r1.Name == "test-suite")
        //                continue;

        //            //porownujemy atrybuty
        //            if ((r1.Name == "test-case" /*|| r1.Name == "test-suite"*/))
        //            {
        //                if (r1.MoveToFirstAttribute() == false || r2.MoveToFirstAttribute() == false) continue;

        //                testnum++;
        //                bool spalony = false;
        //                do
        //                {

        //                    if (r1.Name != r2.Name) result = false;
        //                    if (r1.Name == "executed")
        //                    {
        //                        if (r1.Value != r2.Value)
        //                        {
        //                            result = false;
        //                            if (spalony == false)
        //                            {
        //                                failed++;
        //                                testsList.Add(testnum);
        //                            }
        //                            spalony = true;

        //                        }

        //                    }
        //                    if (r1.Name == "success")
        //                    {
        //                        if (r1.Value != r2.Value)
        //                        {
        //                            result = false;
        //                            if (spalony == false)
        //                            {
        //                                failed++;
        //                                testsList.Add(testnum);
        //                            }
        //                            spalony = true;
        //                        }

        //                    }


        //                } while (r1.MoveToNextAttribute() && r2.MoveToNextAttribute());
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        if (r1 != null) r1.Close();
        //        if (r2 != null) r2.Close();


        //    }
        //    passed = testnum - failed;
        //    return result;
        //}


        //public override TestRunner LoadConfigFile(string path)
        //{
        //    XmlSerializer serial = new XmlSerializer(typeof(TestRunnerNUnit));
        //    TestRunnerNUnit r = serial.Deserialize(new XmlTextReader(path)) as TestRunnerNUnit;
        //    return r;
        //}



        //public override void SaveConfiguration(string path)
        //{
        //    Type type = typeof(TestRunnerNUnit);
        //    XmlSerializer serial = new XmlSerializer(type);
        //    TestRunnerNUnit t = (TestRunnerNUnit)(this);
        //    serial.Serialize(new XmlTextWriter(path, null), t);
        //}
    }
}
