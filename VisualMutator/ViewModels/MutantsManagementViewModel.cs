namespace VisualMutator.ViewModels
{
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Mutations;
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

        private MutationSession _selectedMutant;

        public MutationSession SelectedMutant
        {
            get
            {
                return _selectedMutant;
            }
            set
            {
                SetAndRise(ref _selectedMutant, value, () => SelectedMutant);
            }
        }

        public void Show()
        {
            View.ShowDialog();
        }
    }
}