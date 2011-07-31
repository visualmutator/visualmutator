namespace VisualMutator.MvcMutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Mono.Cecil;
    using Mono.Cecil.Cil;


    using VisualMutator.Extensibility;

    #endregion

    [Export(typeof(IMutationOperator))]
    public class Mutation1 : IMutationOperator
    {
       
        public void Mutate(IEnumerable<TypeDefinition> types)
        {
       //     AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(assemblyPath);
      
            //  var tt = new TypeReference();

            //      var typ = ad.MainModule.Types.Single(t => t.Name == "ShoppingCartController");

            //   var rr = typ, IsType("System.Web.Mvc.Controller");
            //    ad.Modules

            // def.
            var controllers = new List<TypeDefinition>();

            controllers.Add(types
                .FirstOrDefault(t => t.IsOfType("System.Web.Mvc.Controller")));


            
          //  ModuleWriter.WriteModuleTo();

            foreach (TypeDefinition controllerType in controllers)
            {
                MethodDefinition methodToModify = controllerType.Methods
                    .FirstOrDefault(
                        m =>
                        m.ReturnType.FullName == "System.Web.Mvc.ActionResult"
                        && !m.IsAbstract);

                MethodDefinition targetAction = controllerType.Methods
                    .FirstOrDefault(
                        m =>
                        m != methodToModify
                        && m.IsPublic
                        && m.Parameters.Count == 0
                        && m.ReturnType.Resolve().IsOfType("System.Web.Mvc.ActionResult"));

                if (methodToModify != null && targetAction != null)
                {
                    Instruction istr = methodToModify.Body.Instructions
                        .FirstOrDefault(
                            i =>
                            {
                                if (i.OpCode == OpCodes.Call)
                                {
                                    var method = ((MethodReference)i.Operand);
                                    if (method.DeclaringType.FullName == "System.Web.Mvc.Controller"
                                        && method.Name == "View"
                                        && method.Parameters.Count == 0)
                                    {
                                        return true;
                                    }
                                }
                                return false;
                            });

                    ILProcessor proc = methodToModify.Body
                        .GetILProcessor();
              
                    AssemblyNameReference ass =
                        controllerType.Module.AssemblyReferences.Single(x => x.Name == "System.Web.Mvc");

                    AssemblyDefinition def = controllerType.Module.AssemblyResolver.Resolve(ass);

                    var type = new TypeReference(
                        "System.Web.Mvc", "Controller", def.MainModule, ass);

                    var me =
                        def.MainModule.Types.Single(t => t.FullName == "System.Web.Mvc.Controller")
                        .Methods.Single(m => m.FullName == "System.Web.Mvc.RedirectToRouteResult System.Web.Mvc.Controller::RedirectToAction(System.String)");
                //    typ.Module.Import()

                //    var me = new MethodReference("RedirectToAction", type);
                    proc.InsertBefore(istr, Instruction.Create(OpCodes.Ldstr,"Index"));
                    proc.Replace(istr, Instruction.Create(OpCodes.Call, controllerType.Module.Import(me)));
                }
            }

           // ad.Write(assemblyPath);


        }
    }

    internal static class Mixin
    {
        public static bool IsOfType(this TypeDefinition type, string fullName)
        {
            TypeDefinition currentType = type;

            for (int i = 0; i < 10000; i++)
            {
                if (currentType.FullName == "<Module>")
                {
                    return false;
                }
                string str = currentType.BaseType.FullName;
                if (str == fullName)
                {
                    return true;
                }
                if (str == "System.Object")
                {
                    return false;
                }

                try
                {
                    currentType = currentType.BaseType.Resolve();
                }
                catch (AssemblyResolutionException e)
                {
                    return false;
                }
                //TODO: An unhandled exception of type 'Mono.Cecil.AssemblyResolutionException' occurred in Mono.Cecil.dll
                //TODO: Additional information: Failed to resolve assembly: 'Asp.net mvc, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
            }
            throw new InvalidOperationException();
        }
    };
}