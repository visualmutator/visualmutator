namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public class MutationElementProperty: IMutationElement<PropertyDefinition>
    {
        private MonoCecilTypeIdentifier _typeIdentifier;

        private string _propertyFullName;

        public MutationElementProperty(PropertyDefinition property)
        {
            _typeIdentifier = new MonoCecilTypeIdentifier(property.DeclaringType);
            _propertyFullName = property.FullName;
      
        }
        public PropertyDefinition FindIn(ICollection<AssemblyDefinition> assemblies)
        {
            return _typeIdentifier.FindType(assemblies).Properties.Single(f => f.FullName == _propertyFullName );
        }
    }
}