namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    public class TypeNode : TreeElement
    {
        private readonly string _fullName;

        public TypeNode(string fullName, string name)
        {
            _fullName = fullName;

            Name = name;
            IsIncluded = true;
        }

        public string FullName
        {
            get
            {
                return _fullName;
            }
        }
    }
}