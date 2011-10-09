namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Controllers;
    using VisualMutator.Views;

    public class MutationResultsViewModel : ViewModel<IMutationResultsView>
    {
        public MutationResultsViewModel(IMutationResultsView view)
            : base(view)
        {
            
        }

        private bool _areOperationsOngoing;

        public bool AreOperationsOngoing
        {
            get
            {
                return _areOperationsOngoing;
            }
            set
            {
                SetAndRise(ref _areOperationsOngoing, value, () => AreOperationsOngoing);
            }
        }

        private BasicCommand _commandCreateNewMutants;

        public BasicCommand CommandCreateNewMutants
        {
            get
            {
                return _commandCreateNewMutants;
            }
            set
            {
                SetAndRise(ref _commandCreateNewMutants, value, () => CommandCreateNewMutants);
            }
        }


        private BasicCommand _commandStop;

        public BasicCommand CommandStop
        {
            get
            {
                return _commandStop;
            }
            set
            {
                SetAndRise(ref _commandStop, value, () => CommandStop);
            }
        }

        private BetterObservableCollection<ExecutedOperator> _operators;

        public BetterObservableCollection<ExecutedOperator> Operators
        {
            get
            {
                return _operators;
            }
            set
            {
                SetAndRise(ref _operators, value, () => Operators);
            }
        }

        private ExecutedOperator _selectedOperator;

        public ExecutedOperator SelectedOperator
        {
            get
            {
                return _selectedOperator;
            }
            set
            {
                SetAndRise(ref _selectedOperator, value, () => SelectedOperator);
            }
        }

        private Mutant _selectedMutant;

        public Mutant SelectedMutant
        {
            get
            {
                return _selectedMutant;
            }
            set
            {
                SetAndRise(ref _selectedMutant, value, () => SelectedMutant);
            }
        }

        private string _operationsStateDescription;

        public string OperationsStateDescription
        {
            get
            {
                return _operationsStateDescription;
            }
            set
            {
                SetAndRise(ref _operationsStateDescription, value, () => OperationsStateDescription);
            }
        }
    }
}