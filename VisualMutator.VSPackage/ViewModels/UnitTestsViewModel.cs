namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System.Collections.ObjectModel;

    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    using VisualMutator.Domain;

    public class UnitTestsViewModel : ExtViewModel<IUnitTestsView>
    {

        public UnitTestsViewModel(IUnitTestsView view)
            : base(view)
        {


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



    }
}