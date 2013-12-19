namespace VisualMutator.Model
{
    #region

    using Tests.Custom;
    using UsefulTools.Core;

    #endregion

    public class MutantsTestingOptions : ModelElement
    {
        public MutantsTestingOptions()
        {
        TestingProcessExtensionOptions = new TestingProcessExtensionOptions();
        }
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