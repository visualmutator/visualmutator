namespace VisualMutator.ViewModels
{
    using CommonUtilityInfrastructure.WpfUtils;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.Views;

    public class MutantDetailsViewModel: ViewModel<IMutantDetailsView>
    {
        public MutantDetailsViewModel(IMutantDetailsView view)
            : base(view)
        {
            TestNamespaces = new NotifyingCollection<TestNodeNamespace>();

            CodeLanguages = new NotifyingCollection<CodeLanguage> { CodeLanguage.CSharp, CodeLanguage.IL };

            SelectedLanguage = CodeLanguage.CSharp;
        }


        private NotifyingCollection<TestNodeNamespace> _testNamespaces;

        public NotifyingCollection<TestNodeNamespace> TestNamespaces
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

        private NotifyingCollection<CodeLanguage> _codeLanguages;

        public NotifyingCollection<CodeLanguage> CodeLanguages
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