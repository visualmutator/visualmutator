namespace VisualMutator.Model
{
    using CommonUtilityInfrastructure.WpfUtils;
    using VisualMutator.Model.Tests.Custom;

    public class MutantsTestingOptions : ModelElement
    {

        private int _testingTimeoutSeconds;

        public int TestingTimeoutSeconds
        {
            get
            {
                return _testingTimeoutSeconds;
            }
            set
            {
                SetAndRise(ref _testingTimeoutSeconds, value, () => TestingTimeoutSeconds);
            }
        }


        private TestingProcessExtensionOptions _testingProcessExtensionOptions;

        public TestingProcessExtensionOptions TestingProcessExtensionOptions
        {
            get
            {
                return _testingProcessExtensionOptions;
            }
            set
            {
                SetAndRise(ref _testingProcessExtensionOptions, value, () => TestingProcessExtensionOptions);
            }
        }
      

    }
}