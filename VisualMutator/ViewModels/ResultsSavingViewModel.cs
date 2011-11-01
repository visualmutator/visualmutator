namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Views;

    public class ResultsSavingViewModel : ViewModel<IResultsSavingView>
    {
        public ResultsSavingViewModel(IResultsSavingView view)
            : base(view)
        {
        }

        public void Show()
        {
            View.ShowDialog();
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

        private BasicCommand _commandSaveResults;

        public BasicCommand CommandSaveResults
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
    }
}