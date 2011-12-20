using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorsStandard
{
    using System.Collections;
    using System.ComponentModel.Composition;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using VisualMutator.Extensibility;



    public class SwapTrueAndFalse : IMutationOperator
    {
        

        public IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {
            var methods = from type in types
                          from method in type.Methods
                          where method.HasBody
                          select method;

            var boolType = types.First().Module.Import(typeof(string));


           // if()

            foreach (var methodDefinition in methods)
            {
                if (methodDefinition.Body.Variables.Select(v => v.VariableType).Any(t => t.Equals(boolType)))
                {
                            
                }
                            
            }

            /*
                        from va in method.Body.Variables
                        let dd = va.VariableType.
                        from instruction in method.Body.Instructions
                        where instruction.OpCode == OpCodes.Add
                        select new MutationTarget().Add("AddInstr", method, instruction);*/
            return Enumerable.Empty<MutationTarget>();
        }
        public void Mutate(MutationContext context)
        {
            var methodAndInstruction = context.MethodAndInstruction("AddInstr");

            var ilProcessor = methodAndInstruction.Method.Body.GetILProcessor();
            ilProcessor.Replace(methodAndInstruction.Instruction, Instruction.Create(OpCodes.Sub));
        }

        public string Identificator
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                return "Swap True and False";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces every occurence of true and false.";
            }
        }

    }
}
