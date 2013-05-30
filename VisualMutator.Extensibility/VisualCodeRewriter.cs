namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using MethodReference = Microsoft.Cci.MutableCodeModel.MethodReference;
    using System;
    using System.Linq;

    /// <summary>
    /// Internal object of the rewriting phase.
    /// </summary>
    public class VisualCodeRewriter : VisualCodeRewriterBase
    {

        /// <summary>
        /// Collection of objects to be processed. Contains only objects that were not processed yet.
        /// </summary>
        private List<Tuple<object, string/*Type Name*/>> _capturedASTObjects;

        /// <summary>
        /// 
        /// </summary>
        private readonly List<object> _sharedASTObjects;
        private readonly IList<TypeIdentifier> _allowedTypes;
       
        private readonly IOperatorCodeRewriter _rewriter;

        public VisualCodeRewriter(IMetadataHost host, 
                                List<Tuple<object, string>> capturedAstObjects, 
                                List<object> sharedAstObjects, 
                                IList<TypeIdentifier> allowedTypes, 
                                IOperatorCodeRewriter rewriter)
            : base(host, rewriter)
        {
            _capturedASTObjects = capturedAstObjects;
            _sharedASTObjects = sharedAstObjects;
            _allowedTypes = allowedTypes;
            this._rewriter = rewriter;
        }

        /// <summary>
        /// Filters elements so that every one is processed only once. 
        /// This is required to avoid rewriting some object multiple times 
        /// by Rewrite methods taking classes from one hierarchy as parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>True if allowed to rewrite the current object</returns>
        protected override bool Process<T>(T obj)
        {
            string typeName = typeof (T).Name;
            var newList = _capturedASTObjects.Where(t => t.Item1 != obj || t.Item2 != typeName).ToList();
            if (newList.Count != _capturedASTObjects.Count)
            {
                _capturedASTObjects = newList;
                return true;
            }
            return _sharedASTObjects.Contains(obj);
       
        }

        /// <summary>
        /// Do not deeper if type is not allowed - (as requested on mutation testing start)
        /// </summary>
        /// <param name="namespaceTypeDefinition"></param>
        public override void RewriteChildren(NamespaceTypeDefinition namespaceTypeDefinition)
        {

            if (_allowedTypes.Count == 0 || _allowedTypes.Contains(new TypeIdentifier(namespaceTypeDefinition)))
            {
                base.RewriteChildren(namespaceTypeDefinition);
            }
        }

        /// <summary>
        /// Capture current method information
        /// </summary>
        /// <param name="method">Current method</param>
        public override void RewriteChildren(MethodDefinition method)
        {
            _rewriter.MethodEnter(method);
            
            base.RewriteChildren(method);
            
            _rewriter.MethodExit(method);
        }

    }
}