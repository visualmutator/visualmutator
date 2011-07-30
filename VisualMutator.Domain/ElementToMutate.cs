namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ElementToMutate : ExtModel
    {
        public ElementToMutate()
        {
            
        }

        private bool _included;

        public bool Included
        {
            set
            {
                _included = value;
                this.RaisePropertyChangedExt(() => Included);
            }
            get
            {
                return _included;
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