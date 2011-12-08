namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    public class MutationContext
    {
        private MutationTarget _mutationTarget;

        private IList<AssemblyDefinition> _assembliesToMutate;

        public IList<AssemblyDefinition> AssembliesToMutate
        {
            get
            {
                return _assembliesToMutate;
            }
        }

        public MutationTarget MutationTarget
        {
            get
            {
                return _mutationTarget;
            }
        }

        public MutationContext(MutationTarget mutationTarget, IList<AssemblyDefinition> assembliesToMutate)
        {
            _mutationTarget = mutationTarget;
            _assembliesToMutate = assembliesToMutate;
        }

        public TypeDefinition Type(string key)
        {
            return ((MutationElementType)_mutationTarget[key]).FindIn(_assembliesToMutate);
        }
        public MethodDefinition Method(string key)
        {
            return ((MutationElementMethod)_mutationTarget[key]).FindIn(_assembliesToMutate);
            
        }
        public MethodAndInstruction MethodAndInstruction(string key)
        {
            return ((MutationElementMethodAndInstruction)_mutationTarget[key]).FindInstructionIn(_assembliesToMutate);
        }
        public PropertyDefinition Property(string key)
        {
            return ((MutationElementProperty)_mutationTarget[key]).FindIn(_assembliesToMutate);
        }
        public FieldDefinition Field(string key)
        {
            return ((MutationElementField)_mutationTarget[key]).FindIn(_assembliesToMutate);
        }
        public EventDefinition Event(string key)
        {
            return ((MutationElementEvent)_mutationTarget[key]).FindIn(_assembliesToMutate);
        }
    }
}