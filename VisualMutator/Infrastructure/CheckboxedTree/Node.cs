namespace VisualMutator.Infrastructure.CheckboxedTree
{
    using CommonUtilityInfrastructure.WpfUtils;

    public abstract class Node : ModelElement
    {
        protected bool? _isIncluded;
        public bool? IsIncluded
        {
            get
            {
                return _isIncluded;
            }
            set
            {
                SetIsIncluded(value, true, true);
              
            }
        }

        private string _name;

        protected Node(string name)
        {
            _name = name;
        }

        public string Name
        {
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
            get
            {
                return _name;
            }
        }
        //  public abstract void SetIsIncluded(bool? value, bool updateChildren, bool updateParent);
        public void SetIsIncluded(bool? value, bool updateChildren, bool updateParent)
        {
            if (value != _isIncluded)
            {
                _isIncluded = value;

                if (updateChildren && _isIncluded != null)
                {
                    UpdateChildren();
                    
                }

                if (updateParent)
                {
                    UpdateParent();
                }

                RaisePropertyChanged(() => IsIncluded);
            }
        }
        protected abstract void UpdateParent();
        protected abstract void UpdateChildren();
        // /  protected abstract void SetIsIncluded(bool? value);
    }
}