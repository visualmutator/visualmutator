namespace VisualMutator.ViewModels
{
    using System.Collections.ObjectModel;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Mutations;
    using VisualMutator.Views;

    public class MutantsManagementViewModel : ViewModel<IMutantsManagementView>
    {
        private readonly IMutantsContainer _mutantsContainer;

        public MutantsManagementViewModel(
            IMutantsManagementView view,
            IMutantsContainer mutantsContainer)
            :base(view)
        {
            _mutantsContainer = mutantsContainer;
            CommandClose = new BasicCommand(Close);

            Mutants = _mutantsContainer.GeneratedMutants;
        }

        public void Close()
        {
            View.Close();
        }

        private ReadOnlyObservableCollection<StoredMutantInfo> _mutants;

        public ReadOnlyObservableCollection<StoredMutantInfo> Mutants
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

        private StoredMutantInfo _selectedMutant;

        public StoredMutantInfo SelectedMutant
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