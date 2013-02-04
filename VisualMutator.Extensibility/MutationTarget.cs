namespace VisualMutator.Extensibility
{
    

    public class MutationTarget
    {
        private readonly int _counterValue;

        public MutationTarget(int counterValue)
        {
            _counterValue = counterValue;
        }

        public int CounterValue
        {
            get { return _counterValue; }
        }
    }
}