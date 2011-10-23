namespace VisualMutator.ViewModels
{
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.CodeDifference;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.Views;

    public class MutantDetailsViewModel: ViewModel<IMutantDetailsView>
    {
        public MutantDetailsViewModel(IMutantDetailsView view)
            : base(view)
        {
            TestNamespaces = new BetterObservableCollection<TestNodeNamespace>();
        }


        private BetterObservableCollection<TestNodeNamespace> _testNamespaces;

        public BetterObservableCollection<TestNodeNamespace> TestNamespaces
        {
            get
            {
                return _testNamespaces;
            }
            set
            {
                SetAndRise(ref _testNamespaces, value, () => TestNamespaces);
            }
        }




        private string _selectedTabHeader;

        public string SelectedTabHeader
        {
            get
            {
                return _selectedTabHeader;
            }
            set
            {
                SetAndRise(ref _selectedTabHeader, value, () => SelectedTabHeader);
            }
        }

        private bool _isCodeLoading;

        public bool IsCodeLoading
        {
            get
            {
                return _isCodeLoading;
            }
            set
            {
                SetAndRise(ref _isCodeLoading, value, () => IsCodeLoading);
            }
        }
        public void PresentCode(CodeWithDifference codeWithDifference)
        {
            View.PresentCode(codeWithDifference);
            //View.SetText(codeWithDifference.Code);
        }

        public void ClearCode()
        {
            View.ClearCode();
        }
    }
}