namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Mono.Cecil;

    public class TypeIdentifier
    {
        public TypeIdentifier(TypeDefinition type)
        {
            _assemblyName = type.Module.Assembly.Name.Name;
            _typeFullName = type.FullName;
        }

        private string _assemblyName;

        public string AssemblyName
        {
            get
            {
                return _assemblyName;
            }
        }

        private string _typeFullName;

        public string TypeFullName
        {
            get
            {
                return _typeFullName;
            }

        }


        public TypeDefinition FindType(ICollection<AssemblyDefinition> assemblies)
        {//TODO: Problem with assembly fullname - different versions
            var aa = assemblies.Single(a => a.Name.Name == _assemblyName).MainModule;
            return aa.Types.Single(t => t.FullName == _typeFullName);
        }
    }
}