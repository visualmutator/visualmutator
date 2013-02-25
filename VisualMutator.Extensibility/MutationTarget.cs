namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using Microsoft.Cci;


    public class MutationTarget
    {
        private readonly string _name;

        private readonly int _counterValue;
        private readonly int _currentPass;

        private readonly string _passInfo;
        private readonly string _callTypeName;

        public string CallTypeName
        {
            get { return _callTypeName; }
        }

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

        public MutationTarget(string name, int counterValue, int currentPass, string passInfo, string callTypeName)
        {
            _name = name;
            _counterValue = counterValue;
            _currentPass = currentPass;
            _passInfo = passInfo;
            _callTypeName = callTypeName;
        }

        public string Name
        {
            get
            {
                return _name;
            }
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

        public override string ToString()
        {
            return string.Format("MutationTarget: {0} - {1}", _name, _passInfo);
        }
    }
}