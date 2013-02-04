namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    public class VisualCodeTraverser : CodeTraverser
    {
        private readonly IList<TypeIdentifier> _allowedTypesName;

        public VisualCodeTraverser(IList<TypeIdentifier> allowedTypesName)
        {
            _allowedTypesName = allowedTypesName;
        }

        public override void TraverseChildren(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            if (_allowedTypesName.Contains(new TypeIdentifier(namespaceTypeDefinition)))
            {
                base.TraverseChildren(namespaceTypeDefinition);
            }
        }
    }
}