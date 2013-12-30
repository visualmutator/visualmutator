namespace VisualMutator.ViewModels
{
    #region

    using System.Windows;
    using Controllers;
    using Model.Mutations.MutantsTree;
    using UsefulTools.Core;
    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class MainViewModel : ViewModel<IMutationResultsView>
    {
        public MainViewModel(IMutationResultsView view)
            : base(view)
        {

            Operators = new NotifyingCollection<ExecutedOperator>();


        }

        public void Clean()
        {
            Operators.Clear();
            SelectedMutationTreeItem = null;
            MutantsRatio = "";
            MutationScore = "";
            OperationsState = OperationsState.None;
            OperationsStateDescription = "";
           
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

      

        private SmartCommand _commandCreateNewMutants;

        public SmartCommand CommandCreateNewMutants
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

        private SmartCommand _commandTest;

        public SmartCommand CommandTest
        {
            get
            {
                return _commandTest;
            }
            set
            {
                SetAndRise(ref _commandTest, value, () => CommandTest);
            }
        }
        private SmartCommand _commandContinue;

        public SmartCommand CommandContinue
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
        private SmartCommand _commandStop;

        public SmartCommand CommandStop
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

        private SmartCommand _commandOnlyCreateMutants;

        public SmartCommand CommandOnlyCreateMutants
        {
            get
            {
                return _commandOnlyCreateMutants;
            }
            set
            {
                SetAndRise(ref _commandOnlyCreateMutants, value, () => CommandOnlyCreateMutants);
            }
        }
        private SmartCommand _commandPause;

        public SmartCommand CommandPause
        {
            get
            {
                return _commandPause;
            }
            set
            {
                SetAndRise(ref _commandPause, value, () => CommandPause);
            }
        }
        private NotifyingCollection<ExecutedOperator> _operators;

        public NotifyingCollection<ExecutedOperator> Operators
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



        public bool IsVisible
        {

            set
            {
                View.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }



        private double _progress;
        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                SetAndRise(ref _progress, value, () => Progress);
            }
        }

        private bool _isProgressIndeterminate;

        public bool IsProgressIndeterminate
        {
            get
            {
                return _isProgressIndeterminate;
            }
            set
            {
                SetAndRise(ref _isProgressIndeterminate, value, () => IsProgressIndeterminate);
            }
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


        private MutantDetailsViewModel _mutantDetailsViewModel;

        public MutantDetailsViewModel MutantDetailsViewModel
        {
            get
            {
                return _mutantDetailsViewModel;
            }
            set
            {
                SetAndRise(ref _mutantDetailsViewModel, value, () => MutantDetailsViewModel);
            }
        }
        private SmartCommand _commandSaveResults;
        public SmartCommand CommandSaveResults
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