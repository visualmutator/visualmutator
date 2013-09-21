namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using Microsoft.Cci;


    public class MutationTarget
    {
        private string _name;
        private int _counterValue;
        private string _callTypeName;
        private MutationVariant _variant;



        public MutationTarget(MutationVariant variant)
        {
            _variant = variant;
        }

        public string CallTypeName
        {
            get { return _callTypeName; }
            set { _callTypeName = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int CounterValue
        {
            get { return _counterValue; }
            set { _counterValue = value; }
        }

        public string ModuleName { get; set; }
        public MutationVariant Variant
        {
            get { return _variant; }
        }

        public IDictionary<string, int> VariantObjectsIndices { get; set; }
        public IMethodDefinition MethodRaw/*?*/ { get; set; }
        public int MethodIndex { get; set; }
        public IMethodDefinition MethodMutated { get; set; }

        public override string ToString()
        {
            return string.Format("MutationTarget: {0}", _name);
        }
    }
}