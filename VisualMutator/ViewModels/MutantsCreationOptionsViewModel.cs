namespace VisualMutator.ViewModels
{
    #region

    using Microsoft.Win32;
    using Model;
    using UsefulTools.Core;
    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class MutantsCreationOptionsViewModel : ViewModel<IMutantsCreationOptionsView>
    {
        public MutantsCreationOptionsViewModel(IMutantsCreationOptionsView view)
            : base(view)
        {
           Options = new MutantsCreationOptions();
           Options.AdditionalFilesToCopy = new NotifyingCollection<string>();
           Options.IsMutantVerificationEnabled = false;
           Options.MaxNumerOfMutantPerOperator = 100;

           CommandAdditionalFilesToCopy = new SmartCommand(ChooseFiles);
        }


        public void ChooseFiles()
        {
            var dlg = new OpenFileDialog();

            bool? result = dlg.ShowDialog();

            if (result == true)
            {

                Options.AdditionalFilesToCopy.ReplaceRange(dlg.FileNames);
            }
        }
        private MutantsCreationOptions _options;

        public MutantsCreationOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                SetAndRise(ref _options, value, () => Options);
            }
        }
        private SmartCommand _commandAdditionalFilesToCopy;
        public SmartCommand CommandAdditionalFilesToCopy
        {
            get
            {
                return _commandAdditionalFilesToCopy;
            }
            set
            {
                SetAndRise(ref _commandAdditionalFilesToCopy, value, () => CommandAdditionalFilesToCopy);
            }
        }

    }
}