namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TreeElement : ExtModel
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
                this.RaisePropertyChangedExt(() => IsIncluded);
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
                this.RaisePropertyChangedExt(() => Name);
            }
            get
            {
                return _name;
            }
        }
    }
}