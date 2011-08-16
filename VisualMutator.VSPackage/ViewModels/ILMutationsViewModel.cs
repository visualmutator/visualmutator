namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System;
    using System.Collections.Generic;

    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    public class ILMutationsViewModel : PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.ViewModel<IILMutationsView>
    {

        public ILMutationsViewModel(IILMutationsView view)
            : base(view)
        {
            Assemblies = new ObservableCollection<AssemblyNode>();
            IsVisible = false;
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
                    this.RaisePropertyChanged(() => CommandRefresh);
                }
            }
        }

        private ICommand _commandMutate;

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
                    this.RaisePropertyChanged(() => CommandMutate);
                }
            }
        }

        private ObservableCollection<AssemblyNode> _assemblies;

        public ObservableCollection<AssemblyNode> Assemblies
        {
            set
            {
                if (_assemblies != value)
                {
                    _assemblies = value;
                    this.RaisePropertyChanged(() => Assemblies);
                }
         
            }
            get
            {
                return _assemblies;
            }
        }

        private ObservableCollection<OperatorPackage> _mutationPackages;

        public ObservableCollection<OperatorPackage> MutationPackages
        {
            set
            {
                if (_mutationPackages != value)
                {
                    _mutationPackages = value;
                    this.RaisePropertyChanged(() => MutationPackages);
                }
            }
            get
            {
                return _mutationPackages;
            }
        }

        private string _loggedText;

        public string LoggedText
        {
            set
            {
                if (_loggedText != value)
                {
                    _loggedText = value;
                    this.RaisePropertyChanged(() => LoggedText);
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
                return ViewCore.Visibility == Visibility.Visible;
            }
            set
            {
                ViewCore.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}

