namespace VisualMutator.Model.Mutations.Traversal
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.ExtensionMethods;
    using MethodBody = Microsoft.Cci.MutableCodeModel.MethodBody;
    using SourceMethodBody = Microsoft.Cci.MutableCodeModel.SourceMethodBody;

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
        private VisualCodeTraverser _traverser;

        public AstFormatter Formatter
        {
            get { return _formatter; }
        }

        public VisualCodeRewriter(IMetadataHost host, List<AstNode> capturedAstObjects, 
            List<AstNode> sharedAstObjects, MutationFilter filter, 
            IOperatorCodeRewriter rewriter, VisualCodeTraverser traverser)
            : base(host, rewriter)
        {
            _capturedASTObjects = capturedAstObjects;
            _sharedASTObjects = sharedAstObjects;
            _filter = filter;
            _rewriter = rewriter;
            _traverser = traverser;
            //    _rewriter.Parent = this;
            _formatter = new AstFormatter();
        }

        public VisualCodeRewriter(IMetadataHost host, List<AstNode> capturedAstObjects, List<AstNode> capturedAstObjects2,
            List<AstNode> sharedAstObjects, List<AstNode> sharedAstObjects2, MutationFilter filter,
            IOperatorCodeRewriter rewriter, VisualCodeTraverser traverser)
            : base(host, rewriter)
        {
            _capturedASTObjects = capturedAstObjects;
            foreach (AstNode element in capturedAstObjects2)
            {
                _capturedASTObjects.Add(element);
            }
            _sharedASTObjects = sharedAstObjects;
            foreach (AstNode element in sharedAstObjects2)
            {
                _sharedASTObjects.Add(element);
            }
            _filter = filter;
            _rewriter = rewriter;
            _traverser = traverser;
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
        {//File.AppendAllText("C:\\PLIKI\\process-logs.txt", "\nProcessing object: " + Formatter.Format(obj)+"\n");
         //   _log.Debug("Processing object: " + Formatter.Format(obj));
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
                //_rewriter2.MethodEnter(method);
                base.RewriteChildren(method);
                _rewriter.MethodExit(method);
                //_rewriter2.MethodEnter(method);
            }
        }

        public override void RewriteChildren(MethodBody methodBody)
        {
            base.RewriteChildren(methodBody);
        }

        public override IMethodBody Rewrite(IMethodBody methodBody)
        {
            SourceMethodBody smb;
          //  _log.Debug("Tring to rewrite: : " + methodBody.MethodDefinition);
            if (_traverser.MethodBodies.TryGetValue(methodBody, out smb))
            {
                _log.Debug("Rewriting SourceMethodBody: "+ smb);
                return Rewrite(smb);
            }
            else
            {
                return base.Rewrite(methodBody);
            }
        }
    }
}