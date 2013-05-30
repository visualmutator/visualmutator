namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using MethodReference = Microsoft.Cci.MutableCodeModel.MethodReference;
    using System;
    using System.Linq;
    public class VisualCodeRewriter : VisualCodeRewriterBase
    {
        private List<Tuple<object, string/*Type Name*/>> _mutationTargets;
        private readonly List<object> _commonTargetsElements;
        private readonly IList<TypeIdentifier> _allowedTypes;
       
        private IOperatorCodeRewriter rewriter;

        public VisualCodeRewriter(IMetadataHost host, 
								List<Tuple<object, string>> mutationTargets, 
								List<object> commonTargetsElements, 
								IList<TypeIdentifier> allowedTypes, 
								IOperatorCodeRewriter rewriter)
            : base(host, rewriter)
        {
            _mutationTargets = mutationTargets;
            _commonTargetsElements = commonTargetsElements;
            _allowedTypes = allowedTypes;
            this.rewriter = rewriter;
        }

        protected override bool Process<T>(T obj)
        {
            string typeName = typeof (T).Name;
            var newList = _mutationTargets.Where(t => t.Item1 != obj || t.Item2 != typeName).ToList();
            if (newList.Count != _mutationTargets.Count)
            {
                _mutationTargets = newList;
                return true;
            }
            return _commonTargetsElements.Contains(obj);
       
        }

        public override void RewriteChildren(NamespaceTypeDefinition namespaceTypeDefinition)
        {

            if (_allowedTypes.Count == 0 || _allowedTypes.Contains(new TypeIdentifier(namespaceTypeDefinition)))
            {
                base.RewriteChildren(namespaceTypeDefinition);
            }
        }
        public override void RewriteChildren(MethodDefinition method)
        {
            rewriter.MethodEnter(method);
            
            base.RewriteChildren(method);
            
            rewriter.MethodExit(method);
        }

    }
}