namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Cci;

    public class TypeIdentifier
    {
        private readonly string _moduleName;

        public string ModuleName
        {
            get { return _moduleName; }
        }

        private readonly string _typeName;

        public string TypeName
        {
            get { return _typeName; }
        }

        public TypeIdentifier(INamespaceTypeDefinition type)
        {
            var module = type.ContainingUnitNamespace.Unit as IModule;
            if (module == null || module == Dummy.Module || module == Dummy.Assembly)
            {
                throw new Exception(type.Name.Value + " has invalid container.");
            }
            _moduleName = module.ModuleName.Value;
            _typeName = type.Name.Value;
        }

        public INamespaceTypeDefinition FindIn(IEnumerable<IModule> modules)
        {
            return modules.Single(m => m.ModuleName.Value == _moduleName)
                .GetAllTypes().OfType<INamespaceTypeDefinition>().Single(t => t.Name.Value == _typeName);
        }

        protected bool Equals(TypeIdentifier other)
        {
            return string.Equals(_typeName, other._typeName) && _moduleName.Equals(other._moduleName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeIdentifier) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_typeName.GetHashCode()*397) ^ _moduleName.GetHashCode();
            }
        }
    }
}