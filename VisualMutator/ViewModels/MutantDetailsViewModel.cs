namespace VisualMutator.ViewModels
{
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.CodeDifference;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.Views;

    public class MutantDetailsViewModel: ViewModel<IMutantDetailsView>
    {
        public MutantDetailsViewModel(IMutantDetailsView view)
            : base(view)
        {
            TestNamespaces = new BetterObservableCollection<TestNodeNamespace>();

            CodeLanguages = new BetterObservableCollection<CodeLanguage> { CodeLanguage.CSharp, CodeLanguage.IL };

            SelectedLanguage = CodeLanguage.CSharp;
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

        private CodeLanguage _selectedLanguage;

        public CodeLanguage SelectedLanguage
        {
            get
            {
                return _selectedLanguage;
            }
            set
            {
                SetAndRise(ref _selectedLanguage, value, () => SelectedLanguage);
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

        private BetterObservableCollection<CodeLanguage> _codeLanguages;

        public BetterObservableCollection<CodeLanguage> CodeLanguages
        {
            get
            {
                return _codeLanguages;
            }
            set
            {
                SetAndRise(ref _codeLanguages, value, () => CodeLanguages);
            }
        }
    }
}