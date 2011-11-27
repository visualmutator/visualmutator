namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Controllers;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Views;

    public class MutantsTestingOptionsViewModel : ViewModel<IMutantsTestingOptionsView>
    {
        public MutantsTestingOptionsViewModel(IMutantsTestingOptionsView view)
            : base(view)
        {
           Options= new MutantsTestingOptions();
           Options.TestingTimeoutSeconds = 10;
        }


        private MutantsTestingOptions _options;

        public MutantsTestingOptions Options
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
    }
}