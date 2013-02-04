namespace VisualMutator.Model
{
    using System.Collections.Generic;
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


    }
}