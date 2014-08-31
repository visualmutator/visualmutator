namespace VisualMutator.Model.Mutations.Traversal
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.ExtensionMethods;

    #endregion

    /// <summary>
    /// Internal object of the rewriting phase.
    /// </summary>
    public class VisualCodeRewriter : VisualCodeRewriterBase, IVisualCodeRewriter
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Collection of objects to be processed. Contains only objects that were not processed yet.
        /// </summary>
        private List<AstNode> _capturedASTObjects;

        /// <summary>
        /// 
        /// </summary>
        private readonly List<AstNode> _sharedASTObjects;

        private readonly MutationFilter _filter;
       
        private readonly IOperatorCodeRewriter _rewriter;
        private readonly AstFormatter _formatter;
        private bool foundAtLeastOneMatchingObject;

        public AstFormatter Formatter
        {
            get { return _formatter; }
        }

        public VisualCodeRewriter(IMetadataHost host, 
                                List<AstNode> capturedAstObjects,
                                List<AstNode> sharedAstObjects, 
                                MutationFilter filter, 
                                IOperatorCodeRewriter rewriter)
            : base(host, rewriter)
        {
            _capturedASTObjects = capturedAstObjects;
            _sharedASTObjects = sharedAstObjects;
            _filter = filter;
            _rewriter = rewriter;
        //    _rewriter.Parent = this;
            _formatter = new AstFormatter();
        }

        /// <summary>
        /// Filters elements so that every one is processed only once. 
        /// This is required to avoid rewriting some object multiple times 
        /// by Rewrite methods taking multiple classes from one hierarchy as parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>True if allowed to rewrite the current object</returns>
        protected override bool Process<T>(T obj)
        {
            string typeName = typeof(T).Name;
            //We are checking if it is the same object (of course) and if 
            var newList = _capturedASTObjects.WhereNot(t => t.Object == obj).ToList();
            if (newList.Count < _capturedASTObjects.Count)
            {
                _log.Debug("Found object: " + Formatter.Format(obj));
            }
            AstNode firstOrDefault = _capturedASTObjects.FirstOrDefault(t => t.Object == obj);
            if (firstOrDefault != null)
            {
                _log.Info("Found object: " + firstOrDefault + " with callTypeName: " +
                    firstOrDefault.Context.CallTypeName + "while current type name is: " + typeName);
                foundAtLeastOneMatchingObject = true;
            }
            newList = _capturedASTObjects.WhereNot(t => t.Object == obj && t.Context.CallTypeName ==
                    typeName).ToList();
            
            if (newList.Count < _capturedASTObjects.Count)
            {
                _capturedASTObjects = newList;
                return true;
            }
            return _sharedASTObjects.Any(x => x.Object == obj);
       
        }
        public void CheckForUnfoundObjects()
        {
            if(_capturedASTObjects.Any())
            {
                _log.Error("Unfound objects while rewriting: " + string.Join(",", _capturedASTObjects));
                throw new Exception("Unfound objects while rewriting: " + string.Join(",", _capturedASTObjects));
            }
        }
        /// <summary>
        /// Do not deeper if type is not allowed - (as requested on mutation testing start)
        /// </summary>
        /// <param name="namespaceTypeDefinition"></param>
        public override void RewriteChildren(NamespaceTypeDefinition namespaceTypeDefinition)
        {

            if (_filter.Matches(namespaceTypeDefinition))
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
            if (_filter.Matches(method))
            {
                _rewriter.MethodEnter(method);
                base.RewriteChildren(method);
                _rewriter.MethodExit(method);
            }
        }

    }
}