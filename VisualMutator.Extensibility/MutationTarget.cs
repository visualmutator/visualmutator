namespace VisualMutator.Extensibility
{
    using Microsoft.Cci;


    public class MutationTarget
    {
        private readonly int _counterValue;

        public MethodIdentifier Method
        {
            get;
            set;
        }

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