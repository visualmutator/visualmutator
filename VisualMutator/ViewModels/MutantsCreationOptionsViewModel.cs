namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Controllers;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Views;

    public class MutantsCreationOptionsViewModel : ViewModel<IMutantsCreationOptionsView>
    {
        public MutantsCreationOptionsViewModel(IMutantsCreationOptionsView view)
            : base(view)
        {
           Options = new MutantsCreationOptions();
           Options.AdditionalFilesToCopy = new NotifyingCollection<string>();
           Options.IsMutantVerificationEnabled = true;

           CommandAdditionalFilesToCopy = new BasicCommand(ChooseFiles);
        }


        public void ChooseFiles()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();

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
        private BasicCommand _commandAdditionalFilesToCopy;
        public BasicCommand CommandAdditionalFilesToCopy
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