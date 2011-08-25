namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    #region Usings

    using System.Windows.Input;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Tests;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    #endregion

    public class UnitTestsViewModel : ViewModel<IUnitTestsView>
    {
        private bool _areTestsRunning;

        private ICommand _commandRunTests;

        private Infrastructure.BetterObservableCollection<MutationSession> _mutants;

        private string _resultText;

        private MutationSession _selectedMutant;

        private TestTreeNode _selectedTestItem;

        private bool _testCurrentSolution;

        private Infrastructure.BetterObservableCollection<TestNodeNamespace> _testNamespaces;

        public UnitTestsViewModel(IUnitTestsView view, BetterObservableCollection<MutationSession> mutants)
            : base(view)
        {
            TestNamespaces = new Infrastructure.BetterObservableCollection<TestNodeNamespace>();
            _mutants = mutants;
        }

        public Infrastructure.BetterObservableCollection<MutationSession> Mutants
        {
            set
            {
                if (_mutants != value)
                {
                    _mutants = value;
                    RaisePropertyChanged(() => Mutants);
                }
            }
            get
            {
                return _mutants;
            }
        }

        public MutationSession SelectedMutant
        {
            set
            {
                if (_selectedMutant != value)
                {
                    _selectedMutant = value;
                    RaisePropertyChanged(() => SelectedMutant);
                }
            }
            get
            {
                return _selectedMutant;
            }
        }

        public ICommand CommandRunTests
        {
            get
            {
                return _commandRunTests;
            }
            set
            {
                if (_commandRunTests != value)
                {
                    _commandRunTests = value;
                    RaisePropertyChanged(() => CommandRunTests);
                }
            }
        }

        public Infrastructure.BetterObservableCollection<TestNodeNamespace> TestNamespaces
        {
            set
            {
                if (_testNamespaces != value)
                {
                    _testNamespaces = value;
                    RaisePropertyChanged(() => TestNamespaces);
                }
            }
            get
            {
                return _testNamespaces;
            }
        }

        public bool TestCurrentSolution
        {
            set
            {
                if (_testCurrentSolution != value)
                {
                    _testCurrentSolution = value;
                    RaisePropertyChanged(() => TestCurrentSolution);
                }
            }
            get
            {
                return _testCurrentSolution;
            }
        }

        private bool _areTestsLoading;

        public bool AreTestsLoading
        {
            set
            {
                if (_areTestsLoading != value)
                {
                    _areTestsLoading = value;
                    RaisePropertyChanged(() => AreTestsLoading);
                }
            }
            get
            {
                return _areTestsLoading;
            }
        }
        public bool AreTestsRunning
        {
            set
            {
                if (_areTestsRunning != value)
                {
                    _areTestsRunning = value;
                    RaisePropertyChanged(() => AreTestsRunning);
                }
            }
            get
            {
                return _areTestsRunning;
            }
        }

        public TestTreeNode SelectedTestItem
        {
            set
            {
                if (_selectedTestItem != value)
                {
                    _selectedTestItem = value;
                    RaisePropertyChanged(() => SelectedTestItem);
                }
            }
            get
            {
                return _selectedTestItem;
            }
        }

        public string ResultText
        {
            set
            {
                if (_resultText != value)
                {
                    _resultText = value;
                    RaisePropertyChanged(() => ResultText);
                }
            }
            get
            {
                return _resultText;
            }
        }
    }
}