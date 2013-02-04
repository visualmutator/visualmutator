namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    public class VisualCodeTraverser : CodeTraverser
    {
        private readonly IList<TypeIdentifier> _allowedTypesName;
        private readonly VisualCodeVisitor _visitor;

        public VisualCodeTraverser(IList<TypeIdentifier> allowedTypesName)
        {
            _allowedTypesName = allowedTypesName;
            _visitor = (VisualCodeVisitor) PreorderVisitor;
        }

        public override void TraverseChildren(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            if (_allowedTypesName.Contains(new TypeIdentifier(namespaceTypeDefinition)))
            {
                base.TraverseChildren(namespaceTypeDefinition);
            }
        }
        public override void TraverseChildren(IMethodDefinition method)
        {
            _visitor.MethodEnter(method);
            base.TraverseChildren(method);
            _visitor.MethodExit(method);
        }
       
    }
}