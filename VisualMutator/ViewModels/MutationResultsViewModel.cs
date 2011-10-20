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
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.Views;

    public class MutationResultsViewModel : ViewModel<IMutationResultsView>
    {
        public MutationResultsViewModel(IMutationResultsView view)
            : base(view)
        {
            AddListener(() => SelectedMutationTreeItem, SelectedMutationItemChanged);
        }



        private void SelectedMutationItemChanged()
        {
            
            var mutant = SelectedMutationTreeItem as Mutant;
            if (mutant != null)
            {

                if (mutant.TestSession != null)
                {
                    TestNamespaces.ReplaceRange(mutant.TestSession.TestNamespaces);
                }
                else
                {
                    TestNamespaces.Clear();
                    EventListeners.Add(mutant, MutantPropertyChanged);
                }
                var prevMutant = _previousSelectedMutationTreeItem as Mutant;
                if (prevMutant != null)
                {
                    EventListeners.Remove(prevMutant, MutantPropertyChanged);
                }
            }
            _previousSelectedMutationTreeItem = SelectedMutationTreeItem;
        }

        private void MutantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TestSession")
            {
                TestNamespaces.ReplaceRange(((Mutant)sender).TestSession.TestNamespaces);
            }
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

        private BetterObservableCollection<TestNodeNamespace> _testNamespaces;

        public BetterObservableCollection<TestNodeNamespace> TestNamespaces
        {
            get
            {
                return _testNamespaces;
            }
            set
            {
                SetAndRise(ref _testNamespaces, value, () => TestNamespaces);
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

        private object _previousSelectedMutationTreeItem;

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


        private bool _areMutantsBeingCreated;

        public bool AreMutantsBeingCreated
        {
            get
            {
                return _areMutantsBeingCreated;
            }
            set
            {
                SetAndRise(ref _areMutantsBeingCreated, value, () => AreMutantsBeingCreated);
            }
        }

        private bool _isPauseRequested;

     
    }
}