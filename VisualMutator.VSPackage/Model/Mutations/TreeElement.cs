namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;

    #endregion

    public class TreeElement : ModelElement
    {
        private bool _isIncluded;

        private string _name;

        public bool IsIncluded
        {
            set
            {
                _isIncluded = value;
                RaisePropertyChanged(() => IsIncluded);
            }
            get
            {
                return _isIncluded;
            }
        }

        public string Name
        {
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
            get
            {
                return _name;
            }
        }
    }
}