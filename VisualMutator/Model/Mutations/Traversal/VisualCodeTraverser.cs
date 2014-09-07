namespace VisualMutator.Model.Mutations.Traversal
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Tests.Services;
    using SourceMethodBody = Microsoft.Cci.MutableCodeModel.SourceMethodBody;

    #endregion

    public class VisualCodeTraverser : CodeTraverser
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        private readonly MutationFilter _filter;
        private readonly VisualCodeVisitor _visitor;
        private readonly CciModuleSource _module;
        private readonly Dictionary<IMethodBody, SourceMethodBody> _methodBodies;

        public Dictionary<IMethodBody, SourceMethodBody> MethodBodies
        {
            get { return _methodBodies; }
        }

        public VisualCodeTraverser(MutationFilter filter, VisualCodeVisitor visitor, CciModuleSource module)
        {
            _filter = filter;
            _visitor = visitor;
            _module = module;
            PreorderVisitor = visitor;
            _methodBodies = new Dictionary<IMethodBody, SourceMethodBody>();
        }
        public override void Traverse(IMethodBody methodBody)
        {
            var moduleInfo =  _module.ModulesInfo.Single();
            var smb =  new Microsoft.Cci.ILToCodeModel.SourceMethodBody(methodBody, _module.Host, moduleInfo.PdbReader, moduleInfo.LocalScopeProvider, DecompilerOptions.None);
           
            _visitor.MethodBodyEnter(smb);
            Traverse(smb);
            var descriptor = _visitor.MethodBodyExit(smb);
            var targetsDescriptors = _visitor.MutationTargets.Select(t => t.ProcessingContext.Descriptor).ToList();

          //  _log.Debug("Returned :"+ descriptor+" comparing with mutaion targets: "+ targetsDescriptors.MakeString());
            if(targetsDescriptors.Any(a => a.IsContainedIn(descriptor)))
            {
                _log.Debug("Adding method body :" + descriptor );
                _methodBodies.Add(methodBody, smb);
            }
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