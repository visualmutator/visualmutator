namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System.Windows;
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




        private ICommand _commandRefresh;

        public ICommand CommandRefresh
        {
            get
            {
                return _commandRefresh;
            }
            set
            {
                if (_commandRefresh != value)
                {
                    _commandRefresh = value;
                    this.RaisePropertyChangedExt(() => CommandRefresh);
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



    }
}