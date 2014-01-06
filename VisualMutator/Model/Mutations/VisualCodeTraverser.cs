namespace VisualMutator.Extensibility
{
    #region

    using System.Collections.Generic;
    using Microsoft.Cci;
    using Model.Mutations;

    #endregion

    public class VisualCodeTraverser : CodeTraverser
    {
        private readonly IList<TypeIdentifier> _allowedTypes;
        private readonly MutationFilter _filter;
        private readonly VisualCodeVisitor _visitor;

        public VisualCodeTraverser(MutationFilter filter, VisualCodeVisitor visitor)
        {
            _filter = filter;
            _visitor = visitor;
            PreorderVisitor = visitor;
        }
      
        public override void TraverseChildren(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            if (_filter.Matches(namespaceTypeDefinition))
            {
                _visitor.TypeEnter(namespaceTypeDefinition);
                base.TraverseChildren(namespaceTypeDefinition);
                _visitor.TypeExit(namespaceTypeDefinition);
            }
        }
        public override void TraverseChildren(IMethodDefinition method)
        {
            if (_filter.Matches(method))
            {
                _visitor.MethodEnter(method);
                base.TraverseChildren(method);
                _visitor.MethodExit(method);
            }
        }
       
    }
}