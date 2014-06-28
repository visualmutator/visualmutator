namespace VisualMutator.ViewModels
{
    #region

    using System.Windows;
    using Model;
    using Model.Tests.Custom;
    using UsefulTools.DependencyInjection;
    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class MutantsTestingOptionsViewModel : ViewModel<IMutantsTestingOptionsView>
    {

        public MutantsTestingOptionsViewModel(
            IMutantsTestingOptionsView view)
            : base(view)
        {
            Options = new MutantsTestingOptions
            {
                TestingTimeoutSeconds = 20,
                TestingProcessExtensionOptions = TestingProcessExtensionOptions.Default
            };

        }
        public void Initialize(ISessionCreationView parent)
        {
            _parent = parent;
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

        private bool _runTestsToFirstFailed;
        public bool RunTestsToFirstFailed
        {
            get { return _runTestsToFirstFailed; }
            set
            {
                SetAndRise(ref _runTestsToFirstFailed, value, () => RunTestsToFirstFailed);
            }
        }
    }
}