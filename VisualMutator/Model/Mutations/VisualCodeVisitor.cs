namespace VisualMutator.Extensibility
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Microsoft.Cci;
    using Model.Mutations;
    using UsefulTools.ExtensionMethods;

    #endregion

    public class VisualCodeVisitor : VisualCodeVisitorBase, IVisualCodeVisitor
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

       
       

        private readonly List<MutationTarget> _mutationTargets;
        private readonly List<MutationTarget> _sharedTargets;
        //private IMethodDefinition _currentMethod;
        private readonly AstFormatter _formatter;
        private int groupCounter;
        private IMethodDefinition _currentMethodObj;
        private readonly AstProcessor _processor;

        private int[] id = { 1 };
        private string _operatorId;


        public AstProcessor Processor
        {
            get { return _processor; }
        }

        public AstFormatter Formatter
        {
            get { return _formatter; }
        }
       
        public VisualCodeVisitor(string operatorId, IOperatorCodeVisitor visitor, IModule module):base(visitor)
        {
            visitor.Parent = this;
            _processor = new AstProcessor(module);
            _operatorId = operatorId;
            _mutationTargets = new List<MutationTarget>();
            _sharedTargets = new List<MutationTarget>();
           
            _formatter = new AstFormatter();
        }
        // Stores information about object in code model tree
        // All objects are processed. Any object can be processed more than once if is in class hierarchy
        protected override bool Process(object obj)
        {
            _processor.Process(obj);
           
            visitor.VisitAny(obj);
            return true;
        }
        public void MethodEnter(IMethodDefinition method)
        {
            _processor.MethodEnter(method);
            _currentMethodObj = method;
        }

        public void MethodExit(IMethodDefinition method)
        {
            _processor.MethodExit(method);
            _currentMethodObj = null;
        }
        public void TypeEnter(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            _processor.TypeEnter(namespaceTypeDefinition);
        }
        public void TypeExit(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            _processor.TypeExit(namespaceTypeDefinition);
        }
        public void MarkMutationTarget<T>(T obj, IList<MutationVariant> variants )
        {
            Func<int> genId = () => id[0]++;
            if (!_processor.IsCurrentlyProcessed(obj))
            {
                throw new ArgumentException("MarkMutationTarget must be called on current Visit method argument");
            }
            // _log.Debug("MarkMutationTarget: " + TreeObjectsCounter + " - " + Formatter.Format(obj)+" : " + obj.GetHashCode());
            string groupname = "#" + (groupCounter++)+" - "+_formatter.Format(obj);
            foreach (var mutationVariant in variants)
            {
                var mutationTarget = new MutationTarget(mutationVariant)
                {

                    Id = _operatorId+"#" + genId(), 
                    Name = mutationVariant.Signature,
                    ProcessingContext = _processor.CreateProcessingContext<T>(),
                    GroupName = groupname,
                };

                if (mutationTarget.ProcessingContext.Method != null)
                {
                    mutationTarget.MethodRaw = (IMethodDefinition) mutationTarget.ProcessingContext.Method.Object;
                }
                
                _mutationTargets.Add(mutationTarget);
            }
        }


        public virtual void PostProcess()
        {
            foreach (var mutationTarget in _mutationTargets)
            {
                _processor.PostProcess(mutationTarget);
            }
        }

       

        public void MarkSharedTarget<T>(T o)
        {
            var mutationTarget = new MutationTarget( 
                new MutationVariant("", new Dictionary<string, object>()))
                                 {
                                     Name = o.GetType().Name, 
                                     ProcessingContext = _processor.CreateProcessingContext<T>(),
                                 };

            _sharedTargets.Add(mutationTarget);
        }

     


        public IMethodDefinition CurrentMethod
        {
            get
            {
                return _currentMethodObj;
            }
        }
        public List<MutationTarget> MutationTargets
        {
            get
            {
                return _mutationTargets;
            }
        }
        public List<MutationTarget> SharedTargets
        {
            get
            {
                return _sharedTargets;
            }
        }


       
    }
}