namespace VisualMutator.MvcMutations
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Linq;
 
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using VisualMutator.Extensibility;

    #endregion

    [Export(typeof(IMutationOperator))]
    public class ReplaceViewWithRedirectToAction : IMutationOperator
    {
     

        public IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {
            var list = new List<MutationTarget>();
            var controllers = types.Where(t => t.IsOfType("System.Web.Mvc.Controller"));
 
            foreach (var controller in controllers)
            {
                var methodsToModify = controller.Methods
                  .Where(m =>
                      !m.IsAbstract &&
                      m.ReturnType.FullName == "System.Web.Mvc.ActionResult");

             
                foreach (var methodToModify in methodsToModify)
                {

                    MethodDefinition methodToRedirectTo = controller.Methods
                    .FirstOrDefault(m =>
                        m != methodToModify
                        && m.IsPublic
                        && m.Parameters.Count == 0
                        && m.ReturnType.FullName == ("System.Web.Mvc.ActionResult"));

                    if (methodToRedirectTo != null)
                    {
                        list.AddRange(FindValidViewCallInstructions(methodToModify, methodToRedirectTo));
                    }
                }
            }
            
            return list;
        }

        private IEnumerable<MutationTarget> FindValidViewCallInstructions(MethodDefinition methodToModify, 
            MethodDefinition methodToRedirectTo)
        {
    
            return
            from instruction in methodToModify.Body.Instructions
            where instruction.OpCode == OpCodes.Call
            let method = ((MethodReference)instruction.Operand)
            where method.DeclaringType.FullName == "System.Web.Mvc.Controller"
                  && method.Name == "View" && HasProperParameters(methodToModify, instruction, method )
            select new MutationTarget()
            .Add("MethodToModify", methodToModify, instruction)
            .Hidden.Add("MethodToRedirectTo", methodToRedirectTo);
           
     
        }

        public void Mutate(MutationContext context)
        {
            var toModify = context.MethodAndInstruction("MethodToModify");
            MethodDefinition methodToModify = toModify.Method;
            MethodDefinition methodToRedirectTo = context.Method("MethodToRedirectTo");

            methodToModify.Body.SimplifyMacros();

            //  var callInstr = methodToModify.Body.GetInstructionAtOffset(target.InstructionOffset);
            Instruction callInstr = toModify.Instruction;
         

            MethodDefinition redirectToActionMethod = GetRedirectToActionMethod(methodToModify.DeclaringType.Module);


            ILProcessor proc = methodToModify.Body.GetILProcessor();
            var method = (MethodReference)callInstr.Operand;



            foreach (var _ in method.Parameters)
            {
                proc.Remove(callInstr.Previous);
            }

            proc.InsertBefore(callInstr, Instruction.Create(OpCodes.Ldstr, methodToRedirectTo.Name));


            proc.Replace(callInstr, Instruction.Create(OpCodes.Call, 
                methodToModify.DeclaringType.Module.Import(redirectToActionMethod)));

 
            methodToModify.Body.OptimizeMacros();

     
        }



        public MethodDefinition  GetRedirectToActionMethod(ModuleDefinition currentModule)
        {
            var mvcModule = CecilExtensions.GetAspNetMvcModule(currentModule);


            return mvcModule.Types.Single(t => t.FullName == "System.Web.Mvc.Controller")
                    .Methods.Single(m => m.FullName ==
        "System.Web.Mvc.RedirectToRouteResult System.Web.Mvc.Controller::RedirectToAction(System.String)");     
        }



        public string Name
        {
            get
            {
                return "RVRA - Replace View with RedirectToAction";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces previous ActionResult with RedirectToAction.";
            }
        }
      
        private static bool HasProperParameters(MethodDefinition methodToModify, Instruction callInstruction, MethodReference method )
        {
            methodToModify.Body.SimplifyMacros();

            Instruction currentInstr = callInstruction.Previous;

            var validOpcodes = new List<OpCode>
            {
                OpCodes.Ldstr,
                OpCodes.Ldloc,
                OpCodes.Ldarg,
                OpCodes.Ldnull,
            };

            var list = new List<Instruction>();
            foreach (var x in method.Parameters)
            {
                list.Add(currentInstr);
                currentInstr = currentInstr.Previous;
            }

            var sss = list.Select(i => i.OpCode).All(validOpcodes.Contains);

            methodToModify.Body.OptimizeMacros();
            return sss;

        }
    }

  
}