namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public class MutationElementField : IMutationElement<FieldDefinition>
    {
        private TypeIdentifier _typeIdentifier;

        private string _fieldFullName;

        public MutationElementField(FieldDefinition field)
        {
            _typeIdentifier = new TypeIdentifier(field.DeclaringType);
            _fieldFullName = field.FullName;
      
        }
        public FieldDefinition FindIn(ICollection<AssemblyDefinition> assemblies)
        {
            return _typeIdentifier.FindType(assemblies).Fields.Single(f => f.FullName == _fieldFullName );
        }
    }
}