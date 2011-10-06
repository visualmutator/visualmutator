namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

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