namespace VisualMutator.Model.Mutations
{
    using System.Reflection.Emit;

    public interface IAstDescriptor
    {
        bool IsContainedIn(AstDescriptor another);
    }
    public struct DummyDescriptor : IAstDescriptor
    {
        public bool Equals(DummyDescriptor other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DummyDescriptor && Equals((DummyDescriptor) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool IsContainedIn(AstDescriptor another)
        {
            return false;
        }

        public static bool operator ==(DummyDescriptor left, DummyDescriptor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DummyDescriptor left, DummyDescriptor right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return "D";
        }
    }

    public struct AstDescriptor : IAstDescriptor
    {
        private readonly IAstDescriptor _significant;
        private readonly int _index;

        public AstDescriptor(IAstDescriptor significant, int index)
        {
            _significant = significant;
            _index = index;
        }
        public AstDescriptor(int index)
        {
            _significant = new DummyDescriptor();
            _index = index;
        }
        public bool Equals(AstDescriptor other)
        {
            return _significant.Equals(other._significant) && _index == other._index;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AstDescriptor && Equals((AstDescriptor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_significant.GetHashCode()*1397) ^ _index;
            }
        }

        public static bool operator ==(AstDescriptor left, AstDescriptor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AstDescriptor left, AstDescriptor right)
        {
            return !Equals(left, right);
        }

        public bool IsContainedIn(AstDescriptor another)
        {
            return another == this || _significant.IsContainedIn(another);
        }

        public override string ToString()
        {
           return _significant + string.Format("->{0}", _index);
        }

        public AstDescriptor GoDown()
        {
            return new AstDescriptor(this, 0);
        }
        public AstDescriptor GoUp()
        {
            return (AstDescriptor) _significant;
        }

        public AstDescriptor Increment()
        {
            return new AstDescriptor(_significant, _index + 1);
        }
    }
}