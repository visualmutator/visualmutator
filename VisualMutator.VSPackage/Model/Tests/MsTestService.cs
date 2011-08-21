namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using Mono.Cecil;

    public class MsTestService : ITestService
    {
        private IEnumerable<string> _assembliesWithTests;

        public MsTestService()
        {
            
        }
        public IDictionary<string, TestTreeNode> TestMap
        {
            get;
            set;
        }
     
        public IEnumerable<TestNodeNamespace> CreateTree(IEnumerable<MethodDefinition> methods)
        {

            var groupsByNamespace = methods.GroupBy(m => m.DeclaringType)
                .GroupBy(groupByType => groupByType.Key.Namespace);

            var list = new List<TestNodeNamespace>();

            foreach (var namespaceGroup in groupsByNamespace)
            {
                var ns = new TestNodeNamespace
                {
                    Name = namespaceGroup.Key

                };


                foreach (var typeGroup in namespaceGroup)
                {
                    var c = new TestNodeClass
                    {
                        Name = typeGroup.Key.Name,
                    };

                    foreach (var method in typeGroup)
                    {
                        var m = new TestNodeMethod
                        {
                            Name = method.Name
                        };


                        c.TestMethods.Add(m);

                        string id = typeGroup.Key.FullName + "." + method.Name;
                        TestMap.Add(id, m);


                    }
                    ns.TestClasses.Add(c);


                }
                list.Add(ns);

            }
            return list;
        }

        public Task<IEnumerable<TestNodeNamespace>> LoadTests(IEnumerable<string> assemblies)
        {
            return new Task<IEnumerable<TestNodeNamespace>>( () =>
            {
                IEnumerable<string> assembliesWithTests;
                var methods = ScanAssemblies(assemblies, out assembliesWithTests);
                _assembliesWithTests = assembliesWithTests;

                return CreateTree(methods);
            });
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