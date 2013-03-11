namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Cci;
    using Mono.Cecil;

    using VisualMutator.Model.Mutations.Types;

    public class AssemblyScanResult
    {
        public IEnumerable<IMethodDefinition> TestMethods
        {
            get;
            set;
        }

        public IEnumerable<string> AssembliesWithTests
        {
            get;
            set;
        }
    }


    public interface IMsTestLoader
    {
        AssemblyScanResult
            ScanAssemblies(IEnumerable<string> assemblies);
    }
    public class MsTestLoader : IMsTestLoader
    {
      


        public MsTestLoader()
        {
            //_assemblies = assemblies;
        }

        public AssemblyScanResult ScanAssemblies(IEnumerable<string> assemblies)
        {
            var list = new List<IMethodDefinition>();
            var withTests = new List<string>();
            foreach (string assembly in assemblies)
            {
                var methods = ReadTestMethodsFromAssembly(assembly);

                list.AddRange(methods);

                if (methods.Any())
                {
                    withTests.Add(assembly);
                }
            }
         
            return new AssemblyScanResult
            {
                TestMethods = list,
                AssembliesWithTests = withTests
            };
        }
        public IEnumerable<IMethodDefinition> ReadTestMethodsFromAssembly(string assembly)
        {
            IModule module = null;//TODO: jakis inny sposób niż _assemblies.ReadFromStream(assembly);

            return from type in module.GetAllTypes()
                   where type.Attributes.Select(a => a.Type).OfType<INamedTypeDefinition>().Any(a => a.Name.Value
                        == @"Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute")
                   from method in type.Methods
                   where method.Attributes.Select(a => a.Type).OfType<INamedTypeDefinition>().Any(a => a.Name.Value
                        == @"Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute")
                   select method;

         
        }

   
    }

}