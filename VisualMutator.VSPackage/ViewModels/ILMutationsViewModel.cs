namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    #region Usings

    using System.Windows;
    using System.Windows.Input;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;

    #endregion

    public class ILMutationsViewModel : ViewModel<IILMutationsView>
    {
        private BetterObservableCollection<AssemblyNode> _assemblies;

        private ICommand _commandMutate;

        private ICommand _commandRefresh;

        private string _loggedText;

        private BetterObservableCollection<OperatorPackage> _mutationPackages;

        public ILMutationsViewModel(IILMutationsView view)
            : base(view)
        {
            Assemblies = new BetterObservableCollection<AssemblyNode>();
            IsVisible = false;
        }

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
                    RaisePropertyChanged(() => CommandRefresh);
                }
            }
        }

        public ICommand CommandMutate
        {
            get
            {
                return _commandMutate;
            }
            set
            {
                if (_commandMutate != value)
                {
                    _commandMutate = value;
                    RaisePropertyChanged(() => CommandMutate);
                }
            }
        }

        public BetterObservableCollection<AssemblyNode> Assemblies
        {
            set
            {
                if (_assemblies != value)
                {
                    _assemblies = value;
                    RaisePropertyChanged(() => Assemblies);
                }
            }
            get
            {
                return _assemblies;
            }
        }

        public BetterObservableCollection<OperatorPackage> MutationPackages
        {
            set
            {
                if (_mutationPackages != value)
                {
                    _mutationPackages = value;
                    RaisePropertyChanged(() => MutationPackages);
                }
            }
            get
            {
                return _mutationPackages;
            }
        }

        public string LoggedText
        {
            set
            {
                if (_loggedText != value)
                {
                    _loggedText = value;
                    RaisePropertyChanged(() => LoggedText);
                }
            }
            get
            {
                return _loggedText;
            }
        }

        public bool IsVisible
        {
            get
            {
                return View.Visibility == Visibility.Visible;
            }
            set
            {
                View.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}