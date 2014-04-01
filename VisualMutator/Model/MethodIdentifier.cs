namespace VisualMutator.Model
{
    using System.Text.RegularExpressions;

    public class MethodIdentifier
    {
        private readonly string _methodName;
        private readonly string _className;

        public MethodIdentifier(string identifier)
        {
            string withoutParams = GetWithoutParams(identifier);
            int dotIndex = withoutParams.LastIndexOf("..", System.StringComparison.Ordinal);
            if (dotIndex == -1)
            {
                dotIndex = withoutParams.LastIndexOf('.');
            }
            _methodName = identifier.Substring(dotIndex + 1);
            _className = identifier.Substring(0, dotIndex);
        }

        public MethodIdentifier(string className, string method)
        {
            _methodName = method;
            _className = className;
        }

        public string MethodNameWithoutParams
        {
            get
            {
                return GetWithoutParams(_methodName);
            }
        }
        private string GetWithoutParams(string name)
        {
            int index = name.IndexOf('(');
            return index == -1 ? name : name.Substring(0, index);
        }
        public string MethodName
        {
            get { return _methodName; }
        }
        public string ClassSimpleName
        {
            get
            {
                int index = _className.LastIndexOf('.') + 1;
                int endIndex = _className.IndexOf('<', index);
                if (endIndex == -1)
                {
                    endIndex = _className.Length;
                }

                return _className.Substring(index, endIndex - index);
            }
        }
        public string ClassName
        {
            get { return _className; }
        }

        public override string ToString()
        {
            return ClassName +'.'+ MethodName;
        }

        protected bool Equals(MethodIdentifier other)
        {
            return string.Equals(_className, other._className) && string.Equals(_methodName, other._methodName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MethodIdentifier) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_className.GetHashCode()*397) ^ _methodName.GetHashCode();
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