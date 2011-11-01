namespace VisualMutator.Model.Mutations.Structure
{
    using System.Collections.Generic;
    using System.Linq;

    public class ExecutedOperator : MutationNode
    {
        public ExecutedOperator(MutationRootNode parent, string name)
            : base(parent, name, true)
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
    }
}