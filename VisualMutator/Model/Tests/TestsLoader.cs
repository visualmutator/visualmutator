namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using Services;
    using Strilanc.Value;
    using TestsTree;

    public class TestsLoader
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly NUnitXmlTestService _nunit;

        public TestsLoader(NUnitXmlTestService nunit)
        {
            _nunit = nunit;
        }

        public TestsRootNode LoadTests(IList<string> assembliesPaths)
        {
            _log.Info("Loading tests from: "+string.Join(",", assembliesPaths));
            var tasks = new Dictionary<string, Task<May<TestNodeAssembly>>>();
            var testsRootNode = new TestsRootNode();

            ITestsService service1 = _nunit;
            foreach (var path in assembliesPaths)
            {
                string path1 = path;
                Task<May<TestNodeAssembly>> task = Task.Run(() => service1.LoadTests(path1))
                    .ContinueWith(result =>
                    {
                        if (result.Exception != null)
                        {
                            _log.Error(result.Exception);
                            return May.NoValue;
                        }
                        else return result.Result.Bind(context =>
                        {
                            string assemblyName = Path.GetFileNameWithoutExtension(path1);
                            string assemblyPath = path1;
                            var testNodeAssembly = new TestNodeAssembly(testsRootNode, assemblyName);
                            testNodeAssembly.AssemblyPath = assemblyPath;
                            testNodeAssembly.TestsLoadContext = context;

                            IEnumerable<TestNodeNamespace> testNamespaces =
                                GroupTestClasses(context.ClassNodes, testNodeAssembly);

                            testNodeAssembly.Children.AddRange(testNamespaces);
                            return new May<TestNodeAssembly>(testNodeAssembly);
                        });
                    });
                tasks.Add(path, task);

            }
            var testNodeAssemblies = Task.WhenAll(tasks.Values).Result
                .WhereHasValue();

            testsRootNode.Children.AddRange(testNodeAssemblies);
            testsRootNode.State = TestNodeState.Inactive;
            testsRootNode.IsIncluded = true;

            return testsRootNode;
        }

        private static IEnumerable<TestNodeNamespace> GroupTestClasses(
            List<TestNodeClass> classNodes, TestNodeAssembly testNodeAssembly)
        {
            return classNodes
                .GroupBy(classNode => classNode.Namespace)
                .Select(group =>
                {
                    var ns = new TestNodeNamespace(testNodeAssembly, @group.Key);
                    foreach (TestNodeClass nodeClass in @group)
                    {
                        nodeClass.Parent = ns;
                    }

                    ns.Children.AddRange(@group);
                    return ns;
                });
        }
    }
}