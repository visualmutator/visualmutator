namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.DependencyInjection;
    using CommonUtilityInfrastructure.WpfUtils;
    using Model;
    using VisualMutator.Controllers;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests.Custom;
    using VisualMutator.Views;

    public class MutantsTestingOptionsViewModel : ViewModel<IMutantsTestingOptionsView>
    {
        private readonly IFactory<ChooseTestingExtensionViewModel> _chooseTestingExtensionFactory;

        public MutantsTestingOptionsViewModel(
            IMutantsTestingOptionsView view,
            IFactory<ChooseTestingExtensionViewModel> chooseTestingExtensionFactory)
            : base(view)
        {
            _chooseTestingExtensionFactory = chooseTestingExtensionFactory;
            Options = new MutantsTestingOptions
            {
                TestingTimeoutSeconds = 10,
                TestingProcessExtensionOptions = TestingProcessExtensionOptions.Default
            };

            CommandChooseTestingExtension = new BasicCommand(ChooseTestingExtension);
        }
        public void Initialize(ISessionCreationView parent)
        {
            _parent = parent;
        }
        private void ChooseTestingExtension()
        {
            var chooseTestingExtensionViewModel = _chooseTestingExtensionFactory.Create();
            chooseTestingExtensionViewModel.Run(_parent, Options.TestingProcessExtensionOptions);
            if (chooseTestingExtensionViewModel.HasResult)
            {
                Options.TestingProcessExtensionOptions = chooseTestingExtensionViewModel.Result;
            }
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

        private BasicCommand _commandChooseTestingExtension;

        private ISessionCreationView _parent;

        public BasicCommand CommandChooseTestingExtension
        {
            get
            {
                return _commandChooseTestingExtension;
            }
            set
            {
                SetAndRise(ref _commandChooseTestingExtension, value, () => CommandChooseTestingExtension);
            }
        }

       
    }
}