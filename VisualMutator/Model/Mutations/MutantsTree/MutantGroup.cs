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
       

        public MutantGroup(string name, CheckedNode parent)
            : base( name, true)
        {
            Parent = parent;
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
            DisplayedText = "Mutation group: {0}"
                    .Formatted(Name);
        }
        public override string ToString()
        {
            return "Mutation group: {0}"
                    .Formatted(Name);
        }
    }
}