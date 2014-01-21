namespace VisualMutator.ViewModels
{
    #region

    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Tests.TestsTree;
    using UsefulTools.Core;
    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class MutantDetailsViewModel: ViewModel<IMutantDetailsView>
    {
        public MutantDetailsViewModel(IMutantDetailsView view)
            : base(view)
        {
            TestNamespaces = new NotifyingCollection<TestNodeNamespace>();

            CodeLanguages = new NotifyingCollection<CodeLanguage> { CodeLanguage.CSharp, CodeLanguage.IL };

            SelectedLanguage = CodeLanguage.CSharp;

          //  SelectedTabHeader = "Code";
            SelectedIndex = 1;
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
        private int _selectedIndex;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                SetAndRise(ref _selectedIndex, value, () => SelectedIndex);
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