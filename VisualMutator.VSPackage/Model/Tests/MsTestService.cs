namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Xml.Linq;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;

    #endregion

    public class MsTestService : AbstractTestService
    {
        private IEnumerable<string> _assembliesWithTests;

        private string RunMsTest()
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(
                @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\MSTest.exe");

            var arguments = new StringBuilder();
            foreach (string assembly in _assembliesWithTests)
            {
                arguments.Append(@"-testcontainer:" + assembly.InQuotes() + " ");
            }

            string resultsFile = Path.GetTempFileName();
            arguments.Append(@"-resultsfile:" + resultsFile.InQuotes());

            p.StartInfo.Arguments = arguments.ToString();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;

            File.Delete(resultsFile);

            p.Start();

            string s = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return resultsFile;
        }

        private TestStatus TranslateTestResultStatus(string status)
        {
            switch (status)
            {
                case "Passed":
                    return TestStatus.Success;
                case "Failed":
                    return TestStatus.Failure;
                case "Inconclusive":
                    return TestStatus.Inconclusive;
                default:
                    throw new ArgumentException("status");
            }
        }

        public IEnumerable<TestNodeMethod> ReadTestResults(string resultsFile)
        {
            XDocument doc = XDocument.Load(resultsFile);
           
            foreach (XElement testResult in doc.Root.DescendantsAnyNs("UnitTestResult"))
            {
                string value = testResult.Attribute("testId").Value;
                var unitTest = doc.Root.DescendantsAnyNs("UnitTest")
                    .Single(n => n.Attribute("id").Value == value);
                var testMethod = unitTest.ElementAnyNS("TestMethod");

                string methodName = testMethod.Attribute("name").Value;
                string longClassName = testMethod.Attribute("className").Value;

                string fullClassName = longClassName.Substring(0, longClassName.IndexOf(","));

                TestNodeMethod node = (TestNodeMethod)TestMap[fullClassName + "," + methodName];

                node.Status = TranslateTestResultStatus(testResult.Attribute("outcome").Value);

                if (node.Status == TestStatus.Failure)
                {
                    testResult.DescendantsAnyNs("ErrorInfo").Single();
                }

                yield return node;
            }
        }

        public override void RunTests()
        {
            
            string results = RunMsTest();
            var tests = ReadTestResults(results).ToList();

            var groups = tests.GroupBy(t => t.Class.FullName);

            foreach (var methodsGroup in groups)
            {
                string fullClassName = methodsGroup.Key;
                TestNodeClass node = (TestNodeClass)TestMap[fullClassName];

                if (methodsGroup.Any(n => n.Status == TestStatus.Failure))
                {
                    node.Status = TestStatus.Failure;
                }
                else if (methodsGroup.Any(n => n.Status == TestStatus.Inconclusive))
                {
                    node.Status = TestStatus.Inconclusive;
                }
                else
                {
                    node.Status = TestStatus.Success;
                }
            }
          
        }

        public override IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies)
        {
            TestMap.Clear();
            IEnumerable<string> assembliesWithTests;
            var methods = ScanAssemblies(assemblies, out assembliesWithTests);
            _assembliesWithTests = assembliesWithTests;

            return CreateTree(methods);
        }

        public IEnumerable<TestNodeClass> CreateTree(IEnumerable<MethodDefinition> methods)
        {
            var groupsByClass = methods.GroupBy(m => m.DeclaringType);
            //     .GroupBy(groupByType => groupByType.Key.Namespace);

            var list = new List<TestNodeClass>();

            foreach (var typeGroup in groupsByClass)
            {
                var type = typeGroup.Key;
                var c = new TestNodeClass
                {
                    Name = type.Name,
                    Namespace = type.Namespace,
                    FullName = type.FullName,
                };

                foreach (MethodDefinition method in typeGroup)
                {
                    var m = new TestNodeMethod
                    {
                        Name = method.Name,
                        Class = c,
                    };

                    c.TestMethods.Add(m);

                    string id = type.FullName + "," + method.Name;
                    TestMap.Add(id, m);
                }

                TestMap.Add(type.FullName, c);
                list.Add(c);
            }

            return list;
        }

        private IEnumerable<MethodDefinition>
            ScanAssemblies(IEnumerable<string> assemblies, out IEnumerable<string> assembliesWithTests)
        {
            var list = new List<MethodDefinition>();
            var withTests = new List<string>();
            foreach (string assembly in assemblies)
            {
                AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(assembly);
                IEnumerable<TypeDefinition> types =
                    ad.MainModule.Types.Where(
                        t =>
                        t.CustomAttributes.Any(
                            a =>
                            a.AttributeType.FullName ==
                            @"Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute")).ToList();

                var methods = types.SelectMany(t => t.Methods).Where(
                    m => m.CustomAttributes.Any(
                        a =>
                        a.AttributeType.FullName ==
                        @"Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute"));

                list.AddRange(methods);

                if (methods.Any())
                {
                    withTests.Add(assembly);
                }
            }
            assembliesWithTests = withTests;
            return list;
        }
    }
}