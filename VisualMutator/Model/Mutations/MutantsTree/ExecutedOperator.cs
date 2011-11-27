namespace VisualMutator.Model.Mutations.Structure
{
    using System.Collections.Generic;
    using System.Linq;

    using CommonUtilityInfrastructure;

    public class ExecutedOperator : MutationNode
    {
        public ExecutedOperator(string name)
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

        public long MutationTimeMiliseconds { get; set; }

        public long FindTargetsTimeMiliseconds { get; set; }

        public void UpdateDisplayedText()
        {
           DisplayedText = "{0} - Mutants: {1}"
                    .Formatted(Name, Children.Count);
        }
    }
}