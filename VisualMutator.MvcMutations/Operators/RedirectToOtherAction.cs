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
    public class RedirectToOtherAction : IMutationOperator
    {
     

        public IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {



            var selected = from type in types
                        where type.IsOfType("System.Web.Mvc.Controller")
                        from method in type.Methods
                        where !method.IsAbstract && method.ReturnType.Resolve().IsOfType("System.Web.Mvc.ActionResult")
                        from instruction in method.Body.Instructions
                        where instruction.OpCode == OpCodes.Call
                        let methodInvokation = ((MethodReference)instruction.Operand)
                        where methodInvokation.DeclaringType.FullName == "System.Web.Mvc.Controller"
                              && methodInvokation.Name == "RedirectToAction"// && methodInvokation.Parameters.Count == 1
                              select new MethodAndInstruction(method,instruction);// new MutationTarget().Add("RedirectToActionMethod", method, instruction);

         //   return lines;

            var list = new List<MutationTarget>();
            foreach (var pair in selected)
            {
                Instruction current = pair.Instruction.Previous;
                while (current != null)
                {
                    if (current.OpCode == OpCodes.Ldarg_0 && current.Next.OpCode == OpCodes.Ldstr)
                    {
                        list.Add(new MutationTarget().Add("lsStringInstruction", pair.Method, current.Next));
                        break;
                    }
                    current = current.Previous;
                }
            }

            return list;

            /*
            //.Add("MethodToModify", methodToModify, instruction)
          //  .Hidden.Add("MethodToRedirectTo", methodToRedirectTo);

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
            
            return list;*/
        }


        public void Mutate(MutationContext context)
        {
            var toModify = context.MethodAndInstruction("lsStringInstruction");
            MethodDefinition methodToModify = toModify.Method;
      

            methodToModify.Body.SimplifyMacros();

            //  var callInstr = methodToModify.Body.GetInstructionAtOffset(target.InstructionOffset);
   
            Instruction ldStrInstr = toModify.Instruction;

            ILProcessor proc = methodToModify.Body.GetILProcessor();
            proc.Replace(ldStrInstr, Instruction.Create(OpCodes.Ldstr, "MutatedIrrevelantName"));

            methodToModify.Body.OptimizeMacros();
         //   MethodDefinition redirectToActionMethod = GetRedirectToActionMethod(methodToModify.DeclaringType.Module);


            /*
            var method = (MethodReference)callInstr.Operand;


            var crawler = new ILCrawler(proc);


            Instruction ins = crawler.GetParameterLoadInstruction(callInstr, 0);


            foreach (var _ in method.Parameters)
            {
                proc.Remove(callInstr.Previous);
            }

            proc.InsertBefore(callInstr, Instruction.Create(OpCodes.Ldstr, methodToRedirectTo.Name));


            proc.Replace(callInstr, Instruction.Create(OpCodes.Call, 
                methodToModify.DeclaringType.Module.Import(redirectToActionMethod)));

 
            methodToModify.Body.OptimizeMacros();

     */
        }

        public string Identificator
        {
            get
            {
                return "CRAT";
            }
        }

        public string Name
        {
            get
            {
                return "Change RedirectToAction Target";
            }
        }

        public string Description
        {
            get
            {
                return "Changes target action of RedirectToAction method.";
            }
        }
      
       
    }

  
}