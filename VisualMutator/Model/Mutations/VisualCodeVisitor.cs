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
    using UsefulTools.ExtensionMethods;

    #endregion

    public class VisualCodeVisitor : VisualCodeVisitorBase, IVisualCodeVisitor
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected int TreeObjectsCounter { get; set; }
        private object _currentObj;
        protected readonly IDictionary<object, int> AllAstIndices;
        protected readonly IDictionary<int, object> AllAstObjects; 
        private readonly List<MutationTarget> _mutationTargets;
        private readonly List<MutationTarget> _sharedTargets;
        private IMethodDefinition _currentMethod;
        private readonly AstFormatter _formatter;
        protected IModule _traversedModule;
        private int groupCounter;
        public AstFormatter Formatter
        {
            get { return _formatter; }
        }

        public VisualCodeVisitor(IOperatorCodeVisitor visitor, IModule module):base(visitor)
        {
            visitor.Parent = this;
            _traversedModule = module;
            _mutationTargets = new List<MutationTarget>();
            _sharedTargets = new List<MutationTarget>();
            AllAstIndices = new Dictionary<object, int>();
            AllAstObjects = new Dictionary<int, object>();
            _formatter = new AstFormatter();
        }
        // Stores information about object in code model tree
        // All objects are processed. Any object can be processed more than once if is in class hierarchy
        protected override bool Process(object obj)
        {
            TreeObjectsCounter++;
            _currentObj = obj;
            if (!AllAstIndices.ContainsKey(obj))
            {
                if (obj is IMethodDefinition)
                {
                //    Debugger.Break();
                }
                AllAstIndices.Add(obj, TreeObjectsCounter);
                AllAstObjects.Add(TreeObjectsCounter, obj);
            }
            visitor.VisitAny(obj);
            return true;
        }

        public void MarkMutationTarget<T>(T obj, IList<MutationVariant> variants )
        {
            if (!ReferenceEquals(_currentObj, obj))
            {
                throw new ArgumentException("MarkMutationTarget must be called on current Visit method argument");
            }
            // _log.Debug("MarkMutationTarget: " + TreeObjectsCounter + " - " + Formatter.Format(obj)+" : " + obj.GetHashCode());
            string groupname = "#" + (groupCounter++)+" - "+_formatter.Format(obj);
            foreach (var mutationVariant in variants)
            {
                var mutationTarget = new MutationTarget(mutationVariant)
                {
                    Name = mutationVariant.Signature,
                    CounterValue = TreeObjectsCounter,
                    CallTypeName = typeof(T).Name,
                    ModuleName = _traversedModule.Name.Value,
                    GroupName = groupname,
                };

                if (_currentMethod != null)
                {
                    mutationTarget.MethodRaw = _currentMethod;
                }

                _mutationTargets.Add(mutationTarget);
            }
            //new Lookup<>
            
           // _mutationTargets.Add(Tuple.Create(groupname, targets));
        }


        public virtual void PostProcess()
        {
            foreach (var mutationTarget in _mutationTargets)
            {
                //just unspecialize all stored objects
                mutationTarget.Variant.AstObjects = mutationTarget.Variant.AstObjects
                    .ToDictionary(pair => pair.Key, pair => Unspecialize(pair.Value));

                if (mutationTarget.Variant.AstObjects.Values.Any(v => !AllAstIndices.ContainsKey(v)))
                {
                    Debugger.Break();
                }
                //translate objects to their indices that identify them
                mutationTarget.VariantObjectsIndices = mutationTarget.Variant
                    .AstObjects.MapValues((key, val) => AllAstIndices[val]);
                mutationTarget.MethodIndex = AllAstIndices[mutationTarget.MethodRaw];
            }
        }

        private object Unspecialize(object value)
        {
            var methodDefinition = value as IMethodDefinition;
            if(methodDefinition != null)
            {
                return MemberHelper.UninstantiateAndUnspecialize(methodDefinition);
            }
            var fieldDefinition = value as IFieldDefinition;
            if (fieldDefinition != null)
            {
                return MemberHelper.Unspecialize(fieldDefinition);
            }
            var eventDefinition = value as IEventDefinition;
            if (eventDefinition != null)
            {
                return MemberHelper.Unspecialize(eventDefinition);
            }
            var fieldReference = value as IFieldReference;
            if (fieldReference != null)
            {
                return MemberHelper.Unspecialize(fieldReference);
            }
            return value;
        }

        public void MarkSharedTarget<T>(T o)
        {
            var mutationTarget = new MutationTarget( 
                new MutationVariant("", new Dictionary<string, object>()))
                                 {
                                     Name = o.GetType().Name, 
                                     CounterValue = TreeObjectsCounter,
                                     CallTypeName = "",
                                 };

            _sharedTargets.Add(mutationTarget);
        }

        public void MethodEnter(IMethodDefinition method)
        {
            _currentMethod = method;
        }

        public void MethodExit(IMethodDefinition method)
        {
            _currentMethod = null;
        }



        public IMethodDefinition CurrentMethod
        {
            get
            {
                return _currentMethod;
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