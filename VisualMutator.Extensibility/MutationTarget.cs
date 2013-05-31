namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using Microsoft.Cci;


    public class MutationTarget
    {
        private readonly string _name;
        private readonly int _counterValue;
        private readonly string _callTypeName;
        private readonly MutationVariant _variant;



        public MutationTarget(string name, int counterValue, string callTypeName, MutationVariant variant)
        {
            _name = name;
            _counterValue = counterValue;
       
            _callTypeName = callTypeName;
            _variant = variant;
        }



        public int CounterValue
        {
            get { return _counterValue; }
        }

   

        public string CallTypeName
        {
            get
            {
                return _callTypeName;
            }
        }

        public MethodIdentifier Method
        {
            get;
            set;
        }

        public MutationVariant Variant
        {
            get { return _variant; }
        }

        public IDictionary<string, int> VariantObjectsIndices { get; set; }

        public override string ToString()
        {
            return string.Format("MutationTarget: {0}", _name);
        }
    }
}