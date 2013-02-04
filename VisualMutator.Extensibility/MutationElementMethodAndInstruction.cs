namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class MutationElementMethodAndInstruction : MutationElementMethod
    {
        private MonoCecilTypeIdentifier _typeIdentifier;

        private string _methodFullName;

        public MutationElementMethodAndInstruction(MethodDefinition method, int instructionIndex )
            : base(method)
        {
            _typeIdentifier = new MonoCecilTypeIdentifier(method.DeclaringType);
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
        public MethodAndInstruction FindInstructionIn(ICollection<AssemblyDefinition> assemblies)
        {
            var method = FindIn(assemblies);
            var instruction = FindInstructionIn(method);
            return new MethodAndInstruction(method, instruction);
        }

    
    }
}