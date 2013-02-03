namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    public class VisualCodeTraverser : CodeTraverser
    {
        private readonly IList<string> _allowedTypesName;

        public VisualCodeTraverser(IList<string> allowedTypesName)
        {
            _allowedTypesName = allowedTypesName;
        }

        public override void TraverseChildren(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            if (_allowedTypesName.Contains(namespaceTypeDefinition.Name.Value))
            {
                base.TraverseChildren(namespaceTypeDefinition);
            }
        }
    }
}