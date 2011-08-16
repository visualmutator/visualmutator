namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    public class TypeNode : TreeElement
    {
        private string _fullName;

        public string FullName
        {
            get
            {
                return _fullName;
            }
        }

        public TypeNode(string fullName, string name)
        {
            _fullName = fullName;

            Name = name;
            IsIncluded = true;
        }
    }
}