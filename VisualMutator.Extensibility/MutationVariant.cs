namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    public class MutationVariant
    {
        private readonly string _signature;
        private readonly IDictionary<string, object> _astObjects;

        public string Signature
        {
            get { return _signature; }
        }

        public IDictionary<string, object> AstObjects
        {
            get { return _astObjects; }
        }

        public MutationVariant(string signature, IDictionary<string, object> astObjects)
        {
            _signature = signature;
            _astObjects = astObjects;
        }
        public MutationVariant()
        {
            _signature = "";
            _astObjects = new Dictionary<string, object>();
        }
    }
}