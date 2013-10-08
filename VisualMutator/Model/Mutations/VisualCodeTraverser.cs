namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    public class VisualCodeTraverser : CodeTraverser
    {
        private readonly IList<TypeIdentifier> _allowedTypes;
        private readonly VisualCodeVisitor _visitor;

        public VisualCodeTraverser(IList<TypeIdentifier> allowedTypes, VisualCodeVisitor visitor)
        {
            _allowedTypes = allowedTypes;
            _visitor = visitor;
            PreorderVisitor = visitor;
        }
      
        public override void TraverseChildren(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            if (_allowedTypes.Count == 0 || _allowedTypes.Contains(new TypeIdentifier(namespaceTypeDefinition)))
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