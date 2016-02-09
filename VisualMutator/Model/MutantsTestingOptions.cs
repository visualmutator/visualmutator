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
            TestingTimeoutSeconds = 40000;
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


      

    }
}