namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class MutationElementMethod : IMutationElement<MethodDefinition>
    {
        private TypeIdentyfier _typeIdentyfier;

        private string _methodFullName;

        public MutationElementMethod(MethodDefinition method, int instructionIndex = -1)
        {
            _typeIdentyfier = new TypeIdentyfier(method.DeclaringType);
            _methodFullName = method.FullName;
            _instructionIndex = instructionIndex;
        }

        private int _instructionIndex;

        public int InstructionIndex
        {
            get
            {
                return _instructionIndex;
            }
        }

        public Instruction FindInstructionIn(MethodDefinition method)
        {
            return method.Body.Instructions[_instructionIndex];
        }
        public Instruction FindInstructionIn(ICollection<AssemblyDefinition> assemblies)
        {
            return FindIn(assemblies).Body.Instructions[_instructionIndex];
        }

        public MethodDefinition FindIn(ICollection<AssemblyDefinition> assemblies)
        {
            return _typeIdentyfier.FindType(assemblies).Methods.Single(m => m.FullName == _methodFullName);
        }
    }
}