namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    #endregion

    public class AssemblyNode : TreeElement
    {
        private readonly string _fullPath;

        private ObservableCollection<TypeNode> _types;

        public AssemblyNode(string name, string fullPath)
        {
            _fullPath = fullPath;

            Types = new ObservableCollection<TypeNode>();

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

        public ObservableCollection<TypeNode> Types
        {
            set
            {
                _types = value;
                RaisePropertyChanged(() => Types);
            }
            get
            {
                return _types;
            }
        }
    }
}