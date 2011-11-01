namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Controllers;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.Views;

    public class MutationResultsViewModel : ViewModel<IMutationResultsView>
    {
        public MutationResultsViewModel(IMutationResultsView view)
            : base(view)
        {


        }


        private OperationsState _operationsState;

        public OperationsState OperationsState
        {
            get
            {
                return _operationsState;
            }
            set
            {
                SetAndRise(ref _operationsState, value, () => OperationsState);
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

        private BasicCommand _commandContinue;

        public BasicCommand CommandContinue
        {
            get
            {
                return _commandContinue;
            }
            set
            {
                SetAndRise(ref _commandContinue, value, () => CommandContinue);
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


        private bool _isVisible;

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                SetAndRise(ref _isVisible, value, () => IsVisible);
            }
        }



        private double _testingProgress;

        public double TestingProgress
        {
            get
            {
                return _testingProgress;
            }
            set
            {
                SetAndRise(ref _testingProgress, value, () => TestingProgress);
            }
        }

        private double _numberOfMutants;
        private double _currentMutant;
        public void InitTestingProgress(int count)
        {
            _numberOfMutants = count;
            _currentMutant = 0;
            TestingProgress = 0;
        }

        public void UpdateTestingProgress()
        {
            _currentMutant++;
            TestingProgress = (_currentMutant / _numberOfMutants) *100;
          
        }

        private object _selectedMutationTreeItem;

   
        public object SelectedMutationTreeItem
        {
            get
            {
                return _selectedMutationTreeItem;
            }
            set
            {
                SetAndRise(ref _selectedMutationTreeItem, value, () => SelectedMutationTreeItem);
            }
        }


        private string _mutantsRatio;

        public string MutantsRatio
        {
            get
            {
                return _mutantsRatio;
            }
            set
            {
                SetAndRise(ref _mutantsRatio, value, () => MutantsRatio);
            }
        }

        private string _mutationScore;

        public string MutationScore
        {
            get
            {
                return _mutationScore;
            }
            set
            {
                SetAndRise(ref _mutationScore, value, () => MutationScore);
            }
        }


        public MutantDetailsViewModel MutantDetailsViewModel
        {
            get; set; }

        private BasicCommand _commandSaveResults;

        public BasicCommand CommandSaveResults
        {
            get
            {
                return _commandSaveResults;
            }
            set
            {
                SetAndRise(ref _commandSaveResults, value, () => CommandSaveResults);
            }
        }
    }
}