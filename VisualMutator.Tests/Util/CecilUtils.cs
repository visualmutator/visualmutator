namespace VisualMutator.Tests.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public static class CecilUtils
    {
  

        public static MethodDefinition CreateMethodDefinition(string name, TypeDefinition type)
        {
       
            var s = new TypeReference("System", "Void", null, null);

            if (type.Module != null)
            {

                s = type.Module.Import(typeof(void));
            }

            return new MethodDefinition(name, MethodAttributes.Public, s)
            {
                DeclaringType = type,
                
            };
        }

        public static AssemblyDefinition CreateAssembly(string assemblyName, IEnumerable<TypeDefinition> types)
        {
            var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(assemblyName, new Version(4,4)),
                "TestModule", ModuleKind.Console);
            foreach (var typeDefinition in types)
            {
                assembly.MainModule.Types.Add(typeDefinition);
               

            }
            return assembly;
        }
        public static TypeDefinition CreateTypeDefinition(string nameSpace, string name)
        {
            return new TypeDefinition(nameSpace, name, TypeAttributes.Public);
        }
    }
}