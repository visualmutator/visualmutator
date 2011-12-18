namespace VisualMutator.Model.Tests.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure;

    using Mono.Cecil;

    using VisualMutator.Model.Tests.TestsTree;

    using log4net;

    #endregion

    public class MsTestService : ITestService
    {
        private readonly IMsTestWrapper _msTestWrapper;

        private readonly IMsTestLoader _msTestLoader;

   
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MsTestService(IMsTestWrapper msTestWrapper, IMsTestLoader msTestLoader)
        {
            _msTestWrapper = msTestWrapper;
            _msTestLoader = msTestLoader;
        }

        public IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies, MutantTestSession mutantTestSession)
        {
          
         
            var result = _msTestLoader.ScanAssemblies(assemblies);
          
            mutantTestSession.AssembliesWithTests = result.AssembliesWithTests.ToList();
            return CreateTree(result.TestMethods, mutantTestSession);
        }
    
        public IEnumerable<TestNodeClass> CreateTree(IEnumerable<MethodDefinition> methods, MutantTestSession mutantTestSession)
        {
            var groupsByClass = methods.GroupBy(m => m.DeclaringType);
          

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
                    mutantTestSession.TestMap.Add(id, m);
                }

          
                list.Add(c);
            }

            return list;
        }

        public List<TestNodeMethod> RunTests(MutantTestSession mutantTestSession)
        {
            if (mutantTestSession.AssembliesWithTests.Any())//TODO: needed?
            {
                XDocument results = _msTestWrapper.RunMsTest(mutantTestSession.AssembliesWithTests);
                return ReadTestResults(results, mutantTestSession).ToList();
            }
            else
            {
                return new List<TestNodeMethod>();
            }

        }

        public void UnloadTests()
        {
            
        }

        public void Cancel()
        {
            //TODO: throw new NotImplementedException();
        }

        public IEnumerable<TestNodeMethod> ReadTestResults(XDocument doc, MutantTestSession mutantTestSession)
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

                TestNodeMethod node = mutantTestSession.TestMap[fullClassName + "," + methodName];

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