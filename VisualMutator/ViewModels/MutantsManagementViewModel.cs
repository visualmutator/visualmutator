namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;

    using VisualMutator.Views;

    public class MutantsManagementViewModel : ViewModel<IMutantsManagementView>
    {
        public MutantsManagementViewModel(IMutantsManagementView view):base(view)
        {
            
        }

        private BetterObservableCollection<MutationSession> _mutants;

        public BetterObservableCollection<MutationSession> Mutants
        {
            get
            {
                return _mutants;
            }
            set
            {
                SetAndRise(ref _mutants, value, () => Mutants);
            }
        }

        private BasicCommand _commandClose;

        public BasicCommand CommandClose
        {
            get
            {
                return _commandClose;
            }
            set
            {
                SetAndRise(ref _commandClose, value, () => CommandClose);
            }
        }

        private BasicCommand _commandRemove;

        public BasicCommand CommandRemove
        {
            get
            {
                return _commandRemove;
            }
            set
            {
                SetAndRise(ref _commandRemove, value, () => CommandRemove);
            }
        }

        private BasicCommand _commandRemoveAll;

        public BasicCommand CommandRemoveAll
        {
            get
            {
                return _commandRemoveAll;
            }
            set
            {
                SetAndRise(ref _commandRemoveAll, value, () => CommandRemoveAll);
            }
        }

        public void Show()
        {
            View.ShowDialog();
        }
    }
}