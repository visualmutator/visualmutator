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

        private BasicCommand _commandMutate;

        private BasicCommand _commandRefresh;

        private string _loggedText;

        private BetterObservableCollection<PackageNode> _mutationPackages;

        public ILMutationsViewModel(IILMutationsView view)
            : base(view)
        {
            Assemblies = new BetterObservableCollection<AssemblyNode>();
            IsVisible = false;
        }

        public BasicCommand CommandRefresh
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

        public BasicCommand CommandMutate
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

        public BetterObservableCollection<PackageNode> MutationPackages
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

        public void MutationLog(string text)
        {
            View.MutationLog(text);
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

        private bool _areTypesLoading;

        public bool AreTypesLoading
        {
            set
            {
                if (_areTypesLoading != value)
                {
                    _areTypesLoading = value;
                    RaisePropertyChanged(() => AreTypesLoading);
                }
            }
            get
            {
                return _areTypesLoading;
            }
        }

        private bool _isMutationOngoing;

        public bool IsMutationOngoing
        {
            get
            {
                return _isMutationOngoing;
            }
            set
            {
                SetAndRise(ref _isMutationOngoing, value, () => IsMutationOngoing);
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


        private BasicCommand _commandLoadLastMutant;

        public BasicCommand CommandLoadLastMutant
        {
            get
            {
                return _commandLoadLastMutant;
            }
            set
            {
                SetAndRise(ref _commandLoadLastMutant, value, () => CommandLoadLastMutant);
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
        public void ClearMutationLog()
        {
            View.ClearMutationLog();
        }
    }
}