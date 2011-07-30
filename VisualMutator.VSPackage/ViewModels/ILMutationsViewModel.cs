namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    public class ILMutationsViewModel : ExtViewModel<IILMutationsView>
    {

        public ILMutationsViewModel(IILMutationsView view)
            : base(view)
        {
            Assemblies = new ObservableCollection<AssemblyNode>();
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


        private ObservableCollection<AssemblyNode> _assemblies;

        public ObservableCollection<AssemblyNode> Assemblies
        {
            set
            {
                _assemblies = value;
                this.RaisePropertyChangedExt(() => Assemblies);
            }
            get
            {
                return _assemblies;
            }
        }


    }
}

