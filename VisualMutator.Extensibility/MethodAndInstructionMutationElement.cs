namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using CommonUtilityInfrastructure;

    using Mono.Cecil;

    public class MethodAndInstructionMutationElement
    {
        private readonly MutationElementMethod _methodElement;

        public MutationElementMethod MethodElement
        {
            get
            {
                return _methodElement;
            }
        }

        public MethodAndInstructionMutationElement(MutationElementMethod methodElement)
        {
            _methodElement = methodElement;
            Throw.If(methodElement.InstructionIndex == -1, "Instruction information is not stored in this element.");
        }

        public MethodAndInstruction FindIn(ICollection<AssemblyDefinition> assemblies)
        {
            var method = _methodElement.FindIn(assemblies);
            var instruction = _methodElement.FindInstructionIn(method);
            return new MethodAndInstruction(method, instruction);
               
        }
    }
}