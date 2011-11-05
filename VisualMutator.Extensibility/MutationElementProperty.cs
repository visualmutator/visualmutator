namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public class MutationElementProperty: IMutationElement<PropertyDefinition>
    {
        private TypeIdentyfier _typeIdentyfier;

        private string _propertyFullName;

        public MutationElementProperty(PropertyDefinition property)
        {
            _typeIdentyfier = new TypeIdentyfier(property.DeclaringType);
            _propertyFullName = property.FullName;
      
        }
        public PropertyDefinition FindIn(ICollection<AssemblyDefinition> assemblies)
        {
            return _typeIdentyfier.FindType(assemblies).Properties.Single(f => f.FullName == _propertyFullName );
        }
    }
}