namespace VisualMutator.Model.Mutations.MutantsTree
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.ExtensionMethods;

    #endregion

    public class MutantGroup : MutationNode
    {
       

        public MutantGroup(string name, CheckedNode parent, IEnumerable<Mutant> mutants = null)
            : base( name, true)
        {
            Parent = parent;
            if(mutants != null)
            {
                Children.AddRange(mutants);
            }
            UpdateDisplayedText();
        }
        public MutantGroup(string name, IEnumerable<Mutant> mutants)
           : base(name, true)
        {
            Children.AddRange(mutants);
            foreach (var child in Children)
            {
                child.Parent = this;
            }

            UpdateDisplayedText();
        }

        public IEnumerable<Mutant> Mutants
        {
            get
            {
                return Children.Cast<Mutant>();
            }
        }


        private string _displayedText;

     

        public string DisplayedText
        {
            get
            {
                return _displayedText;
            }
            set
            {
                SetAndRise(ref _displayedText, value, () => DisplayedText);
            }
        }

        protected override void SetState(MutantResultState value, bool updateChildren, bool updateParent)
        {
            base.SetState(value, updateChildren, updateParent);
            UpdateDisplayedText();
        }

        public void UpdateDisplayedText()
        {
            DisplayedText = "Group: {0}"
                    .Formatted(Name);
        }
        public override string ToString()
        {
            return "Group: {0}"
                    .Formatted(Name);
        }
    }
}