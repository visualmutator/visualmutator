namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using Microsoft.Cci;
    using Model.Mutations;


    public class MutationTarget
    {
        private MutationVariant _variant;



        public MutationTarget(MutationVariant variant)
        {
            _variant = variant;
        }

       // public string CallTypeName { get; set; }

        public string Name { get; set; }

        public ProcessingContext ProcessingContext { get; set; }

       // public string ModuleName { get; set; }
        public MutationVariant Variant
        {
            get { return _variant; }
        }

        
        public IMethodDefinition MethodRaw/*?*/ { get; set; }
        public int MethodIndex { get; set; }
        public IMethodDefinition MethodMutated { get; set; }

        public string GroupName { get; set; }
        public string NamespaceName { get; set; }
        public string TypeName { get; set; }
        public string Id { get; set; }
        public string OperatorId { get; set; }

        public override string ToString()
        {
            return string.Format("MutationTarget: {0}", Name);
        }
    }
}