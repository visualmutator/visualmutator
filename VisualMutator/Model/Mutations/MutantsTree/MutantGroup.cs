namespace VisualMutator.Model.Mutations.MutantsTree
{
    using System.Collections.Generic;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using VisualMutator.Extensibility;

    public class MutantGroup : MutationNode
    {
       

        public MutantGroup(string name)
            : base( name, true)
        {
           
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
            UpdateDisplayedText();
            base.SetState(value, updateChildren, updateParent);
        }

        public void UpdateDisplayedText()
        {
            DisplayedText = "Mutation group: {0} - Mutants: {1}"
                    .Formatted(Name, Children.Count);
        }
    }
}