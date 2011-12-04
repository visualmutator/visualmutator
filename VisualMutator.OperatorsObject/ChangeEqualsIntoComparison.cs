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

    using OpCodes = Mono.Cecil.Cil.OpCodes;

    public class ChangeEqualsIntoComparison : IMutationOperator
    {

        public IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {
            return from type in types
            from method in type.Methods
            where method.HasBody
            from instruction in method.Body.Instructions
            where instruction.OpCode == OpCodes.Add
            select new MutationTarget().Add("AddInstr", method, instruction);

        }
        public void Mutate(MutationTarget target, IList<AssemblyDefinition> assembliesToMutate)
        {
            var methodAndInstruction = target.MethodAndInstruction("AddInstr").FindIn(assembliesToMutate);

            var ilProcessor = methodAndInstruction.Method.Body.GetILProcessor();
            ilProcessor.Replace(methodAndInstruction.Instruction, Instruction.Create(OpCodes.Sub));
        }

        public string Name
        {
            get
            {
                return "Change Addition Into Substraction";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces every occurence of addition with substaction.";
            }
        }

    }
}
