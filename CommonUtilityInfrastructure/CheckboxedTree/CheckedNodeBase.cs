namespace CommonUtilityInfrastructure.CheckboxedTree
{
    using WpfUtils;

    public abstract class CheckedNodeBase : ModelElement
    {
        protected bool? _isIncluded;
        private string _name;
       
        protected CheckedNodeBase(string name)
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

        public override string ToString()
        {
            return string.Format("{0}, IsIncluded: {1}", Name, IsIncluded);
        }
    }
}