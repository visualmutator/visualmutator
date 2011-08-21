namespace ConsoleApplication2
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    #endregion

    internal class Program
    {
        private static void Main(string[] args)
        {
            RefreshTestList(
                new[]
                {
                    @"C:\Users\SysOp\Documents\Visual Studio 2010\Projects\MusicRename\MusicRename.Tests\bin\Debug\MusicRename.Tests.dll"
                });
        }




        public static void RefreshTestList(IEnumerable<string> assemblies)
        {
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



               // var namespaces = types.Select(t => t.Namespace).Distinct();
                var groupsByNamespace = methods.GroupBy(m => m.DeclaringType)
                    .GroupBy(groupByType => groupByType.Key.Namespace);






            }



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
                        _testMap.Add(id, m);


                    }
                    ns.TestClasses.Add(c);


                }
                _unitTestsVm.TestNamespaces.Add(ns);

            }
                


        }
    }
}