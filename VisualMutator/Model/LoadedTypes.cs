namespace VisualMutator.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensibility;
    using Microsoft.Cci;

    public class LoadedTypes
    {
        private readonly List<INamespaceTypeDefinition> _types;

        public List<INamespaceTypeDefinition> Types
        {
            get { return _types; }
        }

        public LoadedTypes(List<INamespaceTypeDefinition> types)
        {
            _types = types;
        }


        public IList<TypeIdentifier> GetIdentifiers()
        {
            return Types.Select(t => new TypeIdentifier(t)).ToList();
        }
    }
}