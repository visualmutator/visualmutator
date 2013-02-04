namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class MutationElementMethod : IMutationElement<MethodDefinition>
    {
        private MonoCecilTypeIdentifier _typeIdentifier;

        private string _methodFullName;

        public MutationElementMethod(MethodDefinition method)
        {
            _typeIdentifier = new MonoCecilTypeIdentifier(method.DeclaringType);
            _methodFullName = method.FullName;
    
        }

  

        public MethodDefinition FindIn(ICollection<AssemblyDefinition> assemblies)
        {
            return _typeIdentifier.FindType(assemblies).Methods.Single(m => m.FullName == _methodFullName);
        }
    }
}