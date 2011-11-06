namespace VisualMutator.Extensibility
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class MethodAndInstruction
    {
        private readonly MethodDefinition _method;

        private readonly Instruction _instruction;

        public MethodDefinition Method
        {
            get
            {
                return _method;
            }
        }

        public Instruction Instruction
        {
            get
            {
                return _instruction;
            }
        }

        public MethodAndInstruction(MethodDefinition method, Instruction instruction)
        {
            _method = method;
            _instruction = instruction;
        }
    }
}