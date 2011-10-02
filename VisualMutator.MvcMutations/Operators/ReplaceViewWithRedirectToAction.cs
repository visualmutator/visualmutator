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
    public class ReplaceViewWithRedirectToAction : IMutationOperator
    {
        
        class MutationTarget
        {
            public MethodDefinition MethodToModify
            {
                get;
                set;
            }
            public MethodDefinition MethodToRedirectTo
            {
                get;
                set;
            }

            public Instruction InstructionToReplace { get; set; }
        }


        public MutationResultDetails Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types)
        {
             var controllers = types.Where(t => t.IsOfType("System.Web.Mvc.Controller"));
             MethodDefinition me = GetRedirectToActionMethod(module);
            IEnumerable<MutationTarget> mutationTargets = controllers.SelectMany(GetMutationTargets).ToList();
            foreach (MutationTarget target in mutationTargets)
            {
       
                var callInstr = target.InstructionToReplace;

                ILProcessor proc = target.MethodToModify.Body.GetILProcessor();
                var method = (MethodReference)callInstr.Operand;

                
           
                foreach (var t in method.Parameters)
                {
                    proc.Remove(callInstr.Previous);
                }

                proc.InsertBefore(callInstr, Instruction.Create(OpCodes.Ldstr, target.MethodToRedirectTo.Name));
                proc.Replace(callInstr, Instruction.Create(OpCodes.Call, module.Import(me)));
            }

            var result = new MutationResultDetails
            {
               
                ModifiedMethods = mutationTargets.Select(t=> t.MethodToModify.FullName).ToList(),
            };

            return result;

        }


        private IEnumerable<MutationTarget> GetMutationTargets(TypeDefinition controller)
        {
            var methodsToModify = controller.Methods
                   .Where(m =>
                       !m.IsAbstract &&
                       m.ReturnType.FullName == "System.Web.Mvc.ActionResult");

            var list = new List<MutationTarget>();

            foreach (var methodToModify in methodsToModify)
            {
                
                MethodDefinition methodToRedirectTo = controller.Methods
                .FirstOrDefault(m =>
                    m != methodToModify
                    && m.IsPublic
                    && m.Parameters.Count == 0
                    && m.ReturnType.FullName==("System.Web.Mvc.ActionResult"));

                if (methodToRedirectTo != null)
                {
                    foreach (var instr in FindValidViewCallInstruction(methodToModify))
                    {
                        var target = new MutationTarget
                        {
                            MethodToModify = methodToModify,
                            MethodToRedirectTo = methodToRedirectTo,
                            InstructionToReplace = instr
                        };
                        list.Add(target);
                    }
                }
            }

            return list;

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
                return "Replace method return statement.";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces previous ActionResult with RedirectToAction.";
            }
        }

        private static IEnumerable<Instruction> FindValidViewCallInstruction(MethodDefinition methodToModify)
        {
            return from Instruction instruction in methodToModify.Body.Instructions
            where instruction.OpCode == OpCodes.Call
            let method = ((MethodReference)instruction.Operand)
            where method.DeclaringType.FullName == "System.Web.Mvc.Controller"
                  && method.Name == "View" && HasProperParameters(instruction, method, methodToModify)
            select instruction;
           
        }

        private static bool HasProperParameters(Instruction callInstruction, MethodReference method, MethodDefinition methodToModify)
        {

            Instruction currentInstr = callInstruction.Previous;

            var validOpcodes = new List<OpCode>
            {
                OpCodes.Ldstr,
                OpCodes.Ldloc,
                OpCodes.Ldloc_S,
                OpCodes.Ldloc_0,
                OpCodes.Ldloc_1,
                OpCodes.Ldloc_2,
                OpCodes.Ldloc_3,
                OpCodes.Ldarg,
                OpCodes.Ldarg_S,
                OpCodes.Ldarg_0,
                OpCodes.Ldarg_1,
                OpCodes.Ldarg_2,
                OpCodes.Ldarg_3,    
                OpCodes.Ldnull,

            };

            var list = new List<Instruction>();
            foreach (var x in method.Parameters)
            {
                list.Add(currentInstr);
                currentInstr = currentInstr.Previous;
            }

            var sss = list.Select(i => i.OpCode).All(validOpcodes.Contains);

            return sss;

        }
    }

  
}