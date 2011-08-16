namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Tests;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    public class UnitTestsViewModel : PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.ViewModel<IUnitTestsView>
    {

        public UnitTestsViewModel(IUnitTestsView view)
            : base(view)
        {

            TestNamespaces = new ObservableCollection<TestNodeNamespace>();
        }


        private ObservableCollection<MutationSession> _mutants;

        public ObservableCollection<MutationSession> Mutants
        {
            set
            {
                if (_mutants != value)
                {
                    _mutants = value;
                    this.RaisePropertyChanged(() => Mutants);
                }
            }
            get
            {
                return _mutants;
            }
        }




        private MutationSession _selectedMutant;

        public MutationSession SelectedMutant
        {
            set
            {
                if (_selectedMutant != value)
                {
                    _selectedMutant = value;
                    this.RaisePropertyChanged(() => SelectedMutant);
                }
            }
            get
            {
                return _selectedMutant;
            }
        }




        private ICommand _commandRunTests;

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
                    this.RaisePropertyChanged(() => CommandRunTests);
                }
            }
        }

        private ObservableCollection<TestNodeNamespace> _testNamespaces;

       
        public ObservableCollection<TestNodeNamespace> TestNamespaces
        {
            set
            {
                if (_testNamespaces != value)
                {
                    _testNamespaces = value;
                    this.RaisePropertyChanged(() => TestNamespaces);
                }
            }
            get
            {
                return _testNamespaces;
            }
        }



        private bool _testCurrentSolution;

        public bool TestCurrentSolution
        {
            set
            {
                if (_testCurrentSolution != value)
                {
                    _testCurrentSolution = value;
                    this.RaisePropertyChanged(() => TestCurrentSolution);
                }
            }
            get
            {
                return _testCurrentSolution;
            }
        }



        private bool _areTestsRunning;

        public bool AreTestsRunning
        {
            set
            {
                if (_areTestsRunning != value)
                {
                    _areTestsRunning = value;
                    this.RaisePropertyChanged(() => AreTestsRunning);
                }
            }
            get
            {
                return _areTestsRunning;
            }
        }

        private TestTreeNode _selectedTestItem;


        public TestTreeNode SelectedTestItem
        {
            set
            {
                if (_selectedTestItem != value)
                {
                    _selectedTestItem = value;
                    this.RaisePropertyChanged(() => SelectedTestItem);
                }
            }
            get
            {
                return _selectedTestItem;
            }
        }



        private string _resultText;

        public string ResultText
        {
            set
            {
                if (_resultText != value)
                {
                    _resultText = value;
                    this.RaisePropertyChanged(() => ResultText);
                }
            }
            get
            {
                return _resultText;
            }
        }
    }
}