namespace VisualMutator.Model.Mutations.Traversal
{
    #region

    using System.Linq;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;

    #endregion

    public class VisualCodeTraverser : CodeTraverser
    {
        private readonly MutationFilter _filter;
        private readonly VisualCodeVisitor _visitor;
        private readonly CciModuleSource _module;

        public VisualCodeTraverser(MutationFilter filter, VisualCodeVisitor visitor, CciModuleSource module)
        {
            _filter = filter;
            _visitor = visitor;
            _module = module;
            PreorderVisitor = visitor;
        }
        public override void Traverse(IMethodBody methodBody)
        {
            var moduleInfo =  _module.ModulesInfo.Single();
            var smb = new SourceMethodBody(methodBody, _module.Host, moduleInfo.PdbReader, moduleInfo.LocalScopeProvider, DecompilerOptions.None);
            _visitor.MethodBodyEnter(smb);
            Traverse(smb);
            _visitor.MethodBodyExit(smb);
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