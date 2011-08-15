namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.Tests;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    using VisualMutator.Domain;

    public class UnitTestsViewModel : ExtViewModel<IUnitTestsView>
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
                    this.RaisePropertyChangedExt(() => Mutants);
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
                    this.RaisePropertyChangedExt(() => SelectedMutant);
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
                    this.RaisePropertyChangedExt(() => CommandRunTests);
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
                    this.RaisePropertyChangedExt(() => TestNamespaces);
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
                    this.RaisePropertyChangedExt(() => TestCurrentSolution);
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
                    this.RaisePropertyChangedExt(() => AreTestsRunning);
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
                    this.RaisePropertyChangedExt(() => SelectedTestItem);
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
                    this.RaisePropertyChangedExt(() => ResultText);
                }
            }
            get
            {
                return _resultText;
            }
        }
    }
}