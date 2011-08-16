namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    public class AssemblyNode : TreeElement
    {

        private string _fullPath;

        public AssemblyNode(string name, string fullPath)
        {
            _fullPath = fullPath;
      

            Types = new Infrastructure.ObservableCollection<TypeNode>();

            Name = name;
            IsIncluded = true;

        }

        public string FullPath
        {
            get
            {
                return _fullPath;
            }
        }

        //  ObservableCollection<TypeNode> Types 

        private Infrastructure.ObservableCollection<TypeNode> _types;

        public Infrastructure.ObservableCollection<TypeNode> Types
        {
            set
            {
                _types = value;
                this.RaisePropertyChanged(() => Types);
            }
            get
            {
                return _types;
            }
        }
    }
}