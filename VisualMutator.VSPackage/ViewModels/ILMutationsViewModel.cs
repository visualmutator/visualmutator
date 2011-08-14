namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System;
    using System.Collections.Generic;

    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    using VisualMutator.Domain;

    public class ILMutationsViewModel : ExtViewModel<IILMutationsView>
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
                    this.RaisePropertyChangedExt(() => CommandRefresh);
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
                    this.RaisePropertyChangedExt(() => CommandMutate);
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
                    this.RaisePropertyChangedExt(() => Assemblies);
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
                    this.RaisePropertyChangedExt(() => MutationPackages);
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
                    this.RaisePropertyChangedExt(() => LoggedText);
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

