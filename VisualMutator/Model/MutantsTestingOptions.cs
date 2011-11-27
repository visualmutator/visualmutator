namespace VisualMutator.Model.Mutations
{
    using CommonUtilityInfrastructure.WpfUtils;

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
    }
}