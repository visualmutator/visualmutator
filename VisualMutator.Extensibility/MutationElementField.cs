namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public class MutationElementField : IMutationElement<FieldDefinition>
    {
        private TypeIdentyfier _typeIdentyfier;

        private string _fieldFullName;

        public MutationElementField(FieldDefinition field)
        {
            _typeIdentyfier = new TypeIdentyfier(field.DeclaringType);
            _fieldFullName = field.FullName;
      
        }
        public FieldDefinition FindIn(ICollection<AssemblyDefinition> assemblies)
        {
            return _typeIdentyfier.FindType(assemblies).Fields.Single(f => f.FullName == _fieldFullName );
        }
    }
}