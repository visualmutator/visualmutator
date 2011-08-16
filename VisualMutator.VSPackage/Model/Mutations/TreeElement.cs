namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    public class TreeElement : PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.ModelElement
    {
        public TreeElement()
        {
            
        }

        private bool _isIncluded;

        public bool IsIncluded
        {
            set
            {
                _isIncluded = value;
                this.RaisePropertyChanged(() => IsIncluded);
            }
            get
            {
                return _isIncluded;
            }
        }

        private string _name;

        public string Name
        {
            set
            {
                _name = value;
                this.RaisePropertyChanged(() => Name);
            }
            get
            {
                return _name;
            }
        }
    }
}