namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mono.Cecil;

    #endregion

    public class MsTestService : ITestService
    {
        private IEnumerable<string> _assembliesWithTests;

        public IDictionary<string, TestTreeNode> TestMap { get; set; }

        public IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies)
        {
            
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
                    Namespace = type.Namespace
                };

                foreach (MethodDefinition method in typeGroup)
                {
                    var m = new TestNodeMethod
                    {
                        Name = method.Name
                    };

                    c.TestMethods.Add(m);

                    string id = type.FullName + "." + method.Name;
                    TestMap.Add(id, m);
                }

                TestMap.Add(type.FullName, c);
                list.Add(c);
            }

            return list;
        }

        public void RunTests()
        {
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