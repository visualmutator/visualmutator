namespace VisualMutator.ViewModels
{
    #region

    using Model;
    using Model.Tests.Custom;
    using UsefulTools.DependencyInjection;
    using UsefulTools.Wpf;
    using Views;

    #endregion

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
                TestingTimeoutSeconds = 30,
                TestingProcessExtensionOptions = TestingProcessExtensionOptions.Default
            };

            CommandChooseTestingExtension = new SmartCommand(ChooseTestingExtension);
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

        private SmartCommand _commandChooseTestingExtension;

        private ISessionCreationView _parent;

        public SmartCommand CommandChooseTestingExtension
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