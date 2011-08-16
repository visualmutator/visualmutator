namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    #region Usings

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    #endregion

    public class MainWindowViewModel : ViewModel<IMainControl>
    {
        private object _iLMutationsView;

        private object _unitTestsView;

        public MainWindowViewModel(IMainControl view)
            : base(view)
        {
        }

        public object ILMutationsView
        {
            set
            {
                _iLMutationsView = value;
                RaisePropertyChanged(() => ILMutationsView);
            }
            get
            {
                return _iLMutationsView;
            }
        }

        public object UnitTestsView
        {
            set
            {
                _unitTestsView = value;
                RaisePropertyChanged(() => UnitTestsView);
            }
            get
            {
                return _unitTestsView;
            }
        }
    }
}