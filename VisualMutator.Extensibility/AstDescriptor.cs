namespace VisualMutator.Model.Mutations
{
    public struct AstDescriptor
    {
        private readonly int _index;

        public AstDescriptor(int index)
        {
            _index = index;
        }

        private bool Equals(AstDescriptor other)
        {
            return _index == other._index;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AstDescriptor) obj);
        }

        public override int GetHashCode()
        {
            return _index;
        }

        public static bool operator ==(AstDescriptor left, AstDescriptor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AstDescriptor left, AstDescriptor right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Index: {0}", _index);
        }
    }
}