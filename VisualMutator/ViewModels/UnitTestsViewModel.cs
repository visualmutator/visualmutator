namespace VisualMutator.ViewModels
{
    #region Usings

    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.Views.Abstract;

    #endregion

    public class UnitTestsViewModel : ViewModel<IUnitTestsView>
    {
        private bool _areTestsLoading;

        private bool _areTestsRunning;

        private BasicCommand _commandDeleteMutant;

        private ICommand _commandRunTests;

        private ReadOnlyObservableCollection<StoredMutantInfo> _mutants;

        private string _resultText;

        private StoredMutantInfo _selectedMutant;

        private TestTreeNode _selectedTestItem;

        private bool _testCurrentSolution;

        private BetterObservableCollection<TestNodeNamespace> _testNamespaces;

        private BasicCommand _showTestDetails;

        public UnitTestsViewModel(IUnitTestsView view, ReadOnlyObservableCollection<StoredMutantInfo> mutants)
            : base(view)
        {
            TestNamespaces = new BetterObservableCollection<TestNodeNamespace>();
            _mutants = mutants;

            _showTestDetails = new BasicCommand(ShowTestDetails);
            _showTestDetails.ExecuteOnChanged(this, () => SelectedTestItem);

        }
        public void ShowTestDetails()
        {
            var method = SelectedTestItem as TestNodeMethod;
            if (method != null && method.HasResults)
            {
                ResultText = method.Message;
            }
            else
            {
                ResultText = "";
            }
        }





        public ReadOnlyObservableCollection<StoredMutantInfo> Mutants
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

        public StoredMutantInfo SelectedMutant
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

       
        public BasicCommand CommandDeleteMutant
        {
            get
            {
                return _commandDeleteMutant;
            }
            set
            {
                SetAndRise(ref _commandDeleteMutant, value, () => CommandDeleteMutant);
            }
        }

        public BetterObservableCollection<TestNodeNamespace> TestNamespaces
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

        private BasicCommand _commandManageMutants;

        public BasicCommand CommandManageMutants
        {
            get
            {
                return _commandManageMutants;
            }
            set
            {
                SetAndRise(ref _commandManageMutants, value, () => CommandManageMutants);
            }
        }
    }
}