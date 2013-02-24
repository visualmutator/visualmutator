namespace VisualMutator.Model.Mutations.MutantsTree
{
    using System.Collections.Generic;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using VisualMutator.Extensibility;

    public class ExecutedOperator : MutationNode
    {
        private readonly string _identificator;
        private readonly IMutationOperator _mutOperator;

        public IMutationOperator Operator
        {
            get { return _mutOperator; }
        }

        public string Identificator
        {
            get
            {
                return _identificator;
            }
        }

        public ExecutedOperator(string identificator, string name, IMutationOperator mutOperator)
            : base( name, true)
        {
            _identificator = identificator;
            _mutOperator = mutOperator;
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

        public IOperatorCodeVisitor OperatorCodeVisitor { get; set; }

        public void UpdateDisplayedText()
        {
           DisplayedText = "{0} - {1} - Mutants: {2}"
                    .Formatted(_identificator, Name, Children.Count);
        }
    }
}