namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Cci;

    public class MethodIdentifier : TypeIdentifier
    {
        private readonly string _methodSignature;

        public string MethodSignature
        {
            get { return _methodSignature; }
        }

        public MethodIdentifier(IMethodDefinition method)
            : base(method.ContainingTypeDefinition as INamedTypeDefinition)
        {
            _methodSignature = method.ToString();
        }

    

        public override string ToString()
        {
            return string.Format("Identifier: Module: {0}, Type: {1}, Method: {2}", ModuleName, TypeName, MethodSignature);
        }

        protected bool Equals(MethodIdentifier other)
        {
            return base.Equals(other) && string.Equals(_methodSignature, other._methodSignature);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MethodIdentifier) obj);
        }
        public new IMethodDefinition FindIn(IEnumerable<IModule> modules)
        {
           // MemberHelper.
            return base.FindIn(modules).Methods.Single(m => m.ToString() == _methodSignature);

        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ _methodSignature.GetHashCode();
            }
        }

        public static bool operator ==(MethodIdentifier left, MethodIdentifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MethodIdentifier left, MethodIdentifier right)
        {
            return !Equals(left, right);
        }
    }
}