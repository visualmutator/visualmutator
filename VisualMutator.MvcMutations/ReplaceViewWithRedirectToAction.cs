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


        public MutationResultDetails Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types,
            Action<MethodDefinition> operatorProgessLog)
        {
             var controllers = types.Where(t => t.IsOfType("System.Web.Mvc.Controller"));

            IEnumerable<MutationTarget> mutationTargets = controllers.SelectMany(GetMutationTargets);
            foreach (MutationTarget target in mutationTargets)
            {
                operatorProgessLog(target.MethodToModify);
                var istr = target.InstructionToReplace;

                ILProcessor proc = target.MethodToModify.Body.GetILProcessor();

                MethodDefinition me = GetRedirectToActionMethod(module);

                proc.InsertBefore(istr, Instruction.Create(OpCodes.Ldstr, target.MethodToRedirectTo.Name));
                proc.Replace(istr, Instruction.Create(OpCodes.Call, module.Import(me)));
            }

            var result = new MutationResultDetails
            {
                Operator = this,
                ModifiedMethods = mutationTargets.Select(t=> t.MethodToModify).ToList(),
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
                var instr = FindValidViewCallInstruction(methodToModify);

                MethodDefinition methodToRedirectTo = controller.Methods
                .FirstOrDefault(m =>
                    m != methodToModify
                    && m.IsPublic
                    && m.Parameters.Count == 0
                    && m.ReturnType.Resolve().IsOfType("System.Web.Mvc.ActionResult"));

                if (instr != null && methodToRedirectTo != null)
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

            return list;

        }


        public MethodDefinition  GetRedirectToActionMethod(ModuleDefinition currentModule)
        {
            AssemblyNameReference ass =
                      currentModule.AssemblyReferences.Single(x => x.Name == "System.Web.Mvc");

            AssemblyDefinition def = currentModule.AssemblyResolver.Resolve(ass);


            return
                def.MainModule.Types.Single(t => t.FullName == "System.Web.Mvc.Controller")
                    .Methods.Single(m => m.FullName == "System.Web.Mvc.RedirectToRouteResult System.Web.Mvc.Controller::RedirectToAction(System.String)");
                 
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

        private static Instruction FindValidViewCallInstruction(MethodDefinition methodToModify)
        {
            return methodToModify.Body.Instructions
                .FirstOrDefault(i =>
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
      
        }
    }

    internal static class Mixin
    {
        public static bool IsOfType(this TypeDefinition type, string fullName)
        {
            TypeDefinition currentType = type;

            for (int i = 0; i < 10000; i++)
            {
                if (currentType.FullName == "<Module>" || !currentType.IsClass)
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