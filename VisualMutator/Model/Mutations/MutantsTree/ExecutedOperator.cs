namespace VisualMutator.Model.Mutations.MutantsTree
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using Extensibility;
    using UsefulTools.ExtensionMethods;

    #endregion

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

        public IEnumerable<MutantGroup> MutantGroups
        {
            get
            {
                return Children.Cast<MutantGroup>();
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
           DisplayedText = "{0} - {1} - Groups: {2}, Mutants: {3}"
                    .Formatted(_identificator, Name, Children.Count, Children.Sum(c => c.Children.Count));
        }
    }
}