namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Model.Mutations;

    public class MutationVariant
    {
        private readonly string _signature;
        private readonly string _additionalInfo;
        private IDictionary<string, object> _astObjects;
        public IDictionary<string, AstDescriptor> ObjectsIndices
        {
            get;
            set;
        }
        public string Signature
        {
            get { return _signature; }
        }

        public IDictionary<string, object> AstObjects
        {
            get { return _astObjects; }

            set { _astObjects = value; }
        }

        public string AdditionalInfo
        {
            get { return _additionalInfo; }
        }

        public MutationVariant(string signature, object astObject, string additionalInfo = "")
        {
            _signature = signature;
            _astObjects = new Dictionary<string, object>();
            _astObjects.Add("", astObject);
        }

        public MutationVariant(string signature, IDictionary<string, object> astObjects, string additionalInfo = "")
        {
            _signature = signature;
            _additionalInfo = additionalInfo;
            _astObjects = astObjects;
        }
        public MutationVariant()
        {
            _signature = "";
            _additionalInfo = "";
            _astObjects = new Dictionary<string, object>();
        }
    }
}