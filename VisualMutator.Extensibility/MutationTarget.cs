namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using Microsoft.Cci;


    public class MutationTarget
    {
        private readonly int _counterValue;
        private readonly int _currentPass;

        private readonly string _passInfo;

        public string PassInfo
        {
            get
            {
                return _passInfo;
            }
        }

        public MethodIdentifier Method
        {
            get;
            set;
        }

        public MutationTarget(int counterValue, int currentPass, string passInfo)
        {
            _counterValue = counterValue;
            _currentPass = currentPass;
            _passInfo = passInfo;
        }

        public int CounterValue
        {
            get { return _counterValue; }
        }
        public int CurrentPass
        {
            get
            {
                return _currentPass;
            }
        }
    }
}