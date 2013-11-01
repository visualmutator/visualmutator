namespace VisualMutator.ViewModels
{
    #region

    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class ResultsSavingViewModel : ViewModel<IResultsSavingView>
    {
        public ResultsSavingViewModel(IResultsSavingView view)
            : base(view)
        {
            IncludeDetailedTestResults = false;
            IncludeCodeDifferenceListings= false;
        }

        public void Show()
        {
            View.SetDefaultOwnerAndShowDialog();
        }

        public void Close()
        {
            View.Close();
        }

        private string _targetPath;

        public string TargetPath
        {
            get
            {
                return _targetPath;
            }
            set
            {
                SetAndRise(ref _targetPath, value, () => TargetPath);
            }
        }

        private bool _includeDetailedTestResults;

        public bool IncludeDetailedTestResults
        {
            get
            {
                return _includeDetailedTestResults;
            }
            set
            {
                SetAndRise(ref _includeDetailedTestResults, value, () => IncludeDetailedTestResults);
            }
        }

        private bool _includeCodeDifferenceListings;

        public bool IncludeCodeDifferenceListings
        {
            get
            {
                return _includeCodeDifferenceListings;
            }
            set
            {
                SetAndRise(ref _includeCodeDifferenceListings, value, () => IncludeCodeDifferenceListings);
            }
        }
        private SmartCommand _commandSaveResults;

        public SmartCommand CommandSaveResults
        {
            get
            {
                return _commandSaveResults;
            }
            set
            {
                SetAndRise(ref _commandSaveResults, value, () => CommandSaveResults);
            }
        }

        private SmartCommand _commandClose;

        public SmartCommand CommandClose
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

        private SmartCommand _commandBrowse;

        public SmartCommand CommandBrowse
        {
            get
            {
                return _commandBrowse;
            }
            set
            {
                SetAndRise(ref _commandBrowse, value, () => CommandBrowse);
            }
        }
    }
}