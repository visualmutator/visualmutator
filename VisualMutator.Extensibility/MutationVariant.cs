namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    public class MutationVariant
    {
        private readonly string _signature;

        public string Signature
        {
            get { return _signature; }
        }

        public IDictionary<string, object> AstObjects { get; set; }

        public MutationVariant(string signature, object astObject)
        {
            _signature = signature;
            AstObjects = new Dictionary<string, object>();
            AstObjects.Add("", astObject);
        }

        public MutationVariant(string signature, IDictionary<string, object> astObjects)
        {
            _signature = signature;
            AstObjects = astObjects;
        }
        public MutationVariant()
        {
            _signature = "";
            AstObjects = new Dictionary<string, object>();
        }
    }
}