namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Xml.Linq;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;

    using log4net;

    #endregion

    public class MsTestService : AbstractTestService
    {
        private readonly IMsTestWrapper _msTestWrapper;

        private readonly IMsTestLoader _msTestLoader;

        private IEnumerable<string> _assembliesWithTests;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MsTestService(IMsTestWrapper msTestWrapper, IMsTestLoader msTestLoader)
        {
            _msTestWrapper = msTestWrapper;
            _msTestLoader = msTestLoader;
        }

        public override IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies)
        {
            TestMap.Clear();
         
            var result = _msTestLoader.ScanAssemblies(assemblies);
            _assembliesWithTests = result.AssembliesWithTests;

            return CreateTree(result.TestMethods);
        }
        /*
                private IEnumerable<MethodDefinition>
                    ScanAssemblies(IEnumerable<string> assemblies, out IEnumerable<string> assembliesWithTests)
                {
                    var list = new List<MethodDefinition>();
                    var withTests = new List<string>();
                    foreach (string assembly in assemblies)
                    {
                        var methods = _msTestWrapper.ReadTestMethodsFromAssembly(assembly);

                        list.AddRange(methods);

                        if (methods.Any())
                        {
                            withTests.Add(assembly);
                        }
                    }
                    assembliesWithTests = withTests;
                    return list;
                }
                */
        public IEnumerable<TestNodeClass> CreateTree(IEnumerable<MethodDefinition> methods)
        {
            var groupsByClass = methods.GroupBy(m => m.DeclaringType);
            //     .GroupBy(groupByType => groupByType.Key.Namespace);

            var list = new List<TestNodeClass>();

            foreach (var typeGroup in groupsByClass)
            {
                var type = typeGroup.Key;
                var c = new TestNodeClass(type.Name)
                {
                    Namespace = type.Namespace,
                    FullName = type.FullName,
                };

                foreach (MethodDefinition method in typeGroup)
                {
                    var m = new TestNodeMethod(c, method.Name);

                    c.Children.Add(m);

                    string id = type.FullName + "," + method.Name;
                    TestMap.Add(id, m);
                }

                //TestMap.Add(type.FullName, c);
                list.Add(c);
            }

            return list;
        }

        public override List<TestNodeMethod> RunTests()
        {
            if (_assembliesWithTests.Any())
            {
                XDocument results = _msTestWrapper.RunMsTest(_assembliesWithTests);
                return ReadTestResults(results).ToList();
            }
            else
            {
                return new List<TestNodeMethod>();
            }

        }
        public IEnumerable<TestNodeMethod> ReadTestResults(XDocument doc)
        {
       
            foreach (XElement testResult in doc.Root.DescendantsAnyNs("UnitTestResult"))
            {
                string value = testResult.Attribute("testId").Value;
                var unitTest = doc.Root.DescendantsAnyNs("UnitTest")
                    .Single(n => n.Attribute("id").Value == value);
                var testMethod = unitTest.ElementAnyNS("TestMethod");

                string methodName = testMethod.Attribute("name").Value;
                string longClassName = testMethod.Attribute("className").Value;

                string fullClassName = longClassName.Substring(0, longClassName.IndexOf(","));

                TestNodeMethod node = TestMap[fullClassName + "," + methodName];

                node.State = TranslateTestResultStatus(testResult.Attribute("outcome").Value);

                if (node.State == TestNodeState.Failure)
                {
                    var errorInfo =testResult.DescendantsAnyNs("ErrorInfo").Single();
                    node.Message = errorInfo.ElementAnyNS("Message").Value;
                }

                yield return node;
            }
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