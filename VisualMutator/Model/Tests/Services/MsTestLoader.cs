namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    using VisualMutator.Model.Mutations.Types;

    public class AssemblyScanResult
    {
        public IEnumerable<MethodDefinition> TestMethods
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
        private readonly IAssemblyReaderWriter _assemblyReaderWriter;

        public MsTestLoader(IAssemblyReaderWriter assemblyReaderWriter)
        {
            _assemblyReaderWriter = assemblyReaderWriter;
        }

        public AssemblyScanResult ScanAssemblies(IEnumerable<string> assemblies)
        {
            var list = new List<MethodDefinition>();
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
        public IEnumerable<MethodDefinition> ReadTestMethodsFromAssembly(string assembly)
        {
            AssemblyDefinition ad = _assemblyReaderWriter.ReadAssembly(assembly);
            IEnumerable<TypeDefinition> types =
                ad.MainModule.Types.Where(
                    t =>
                    t.CustomAttributes.Any(
                        a =>
                        a.AttributeType.FullName ==
                        @"Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute")).ToList();

            return types.SelectMany(t => t.Methods).Where(
                m => m.CustomAttributes.Any(
                    a =>
                    a.AttributeType.FullName ==
                    @"Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute"));
        }

   
    }

}