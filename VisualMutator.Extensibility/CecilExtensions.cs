namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    public static class CecilExtensions
    {
        public static bool IsOfType(this TypeDefinition type, string fullName)
        {
            TypeDefinition currentType = type;

            for (int i = 0; i < 10000; i++)
            {
                if (currentType.FullName == "<Module>" || !currentType.IsClass || currentType.FullName == "System.Object")
                {
                    return false;
                }
                if (currentType.FullName == fullName)
                {
                    return true;
                }

                try
                {
                    currentType = currentType.BaseType.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    return false;
                }

            }
            throw new InvalidOperationException();
        }
        public static Instruction GoBackBy(this Instruction baseinstr, int number)
        {

            Instruction instr = baseinstr;
            for (int i = 0; i < number; i++)
            {
                Throw.If(instr == null);
                instr = instr.Previous;

            }
            return instr;

        }
        public static Instruction GoForwardBy(this Instruction baseinstr, int number)
        {

            Instruction instr = baseinstr;
            for (int i = 0; i < number; i++)
            {
                Throw.If(instr == null);
                instr = instr.Next;
          
            }
            return instr;

        }
  
        public static Instruction GetInstructionAtOffset(this MethodBody body, int offset)
        {
            
            return body.Instructions.Single(i => i.Offset == offset);
        }

        public static ModuleDefinition GetAspNetMvcModule(ModuleDefinition currentModule)
        {
            var mvcModules = currentModule.AssemblyReferences.Where(x => x.Name == "System.Web.Mvc").ToList();
            AssemblyNameReference refer =
                mvcModules.FirstOrDefault(x => x.Version == Version.Parse("3.0.0.0"))
             ?? mvcModules.FirstOrDefault(x => x.Version == Version.Parse("2.0.0.0"));
            if (refer == null)
            {
                throw new ReferencedAssemblyNotFoundException("Valid ASP.NET MVC assembly is not referenced by the project. Only versions 2 and 3 are supported.");
            }
       

            return currentModule.AssemblyResolver.Resolve(refer).MainModule;

        }
    }
}