namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using System;
    using log4net;

    public class VisualCodeVisitor : VisualCodeVisitorBase
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected int TreeObjectsCounter { get; set; }
        private object _currentObj;
        protected readonly IDictionary<object, int> AllAstIndices;
        protected readonly IDictionary<int, object> AllAstObjects; 
        private readonly List<Tuple<string/*GroupName*/, List<MutationTarget>>> _mutationTargets;
        private readonly List<MutationTarget> _sharedTargets;
        private IMethodDefinition _currentMethod;
        private readonly AstFormatter _formatter;

        public AstFormatter Formatter
        {
            get { return _formatter; }
        }

        public VisualCodeVisitor(IOperatorCodeVisitor visitor):base(visitor)
        {
            visitor.Parent = this;
               _mutationTargets = new List<Tuple<string/*GroupName*/, List<MutationTarget>>>();
            _sharedTargets = new List<MutationTarget>();
            AllAstIndices = new Dictionary<object, int>();
            AllAstObjects = new Dictionary<int, object>();
            _formatter = new AstFormatter();
        }

        protected override bool Process(object obj)
        {
            
            TreeObjectsCounter++;
            _currentObj = obj;
            if (!AllAstIndices.ContainsKey(obj))
            {
                AllAstIndices.Add(obj, TreeObjectsCounter);
                AllAstObjects.Add(TreeObjectsCounter, obj);
            }
            this.visitor.VisitAny(obj);
            return true;
        }

        public void MarkMutationTarget<T>(T obj, IList<MutationVariant> variants )
        {
            if (!ReferenceEquals(_currentObj, obj))
            {
                throw new ArgumentException("MarkMutationTarget must be called on current Visit method argument");
            }
            _log.Warn("MarkMutationTarget: " + TreeObjectsCounter + " - " + Formatter.Format(obj)+" : " + obj.GetHashCode());
            var targets = new List<MutationTarget>();
            foreach (var mutationVariant in variants)
            {
                
                var mutationTarget = new MutationTarget(mutationVariant.Signature, 
                    TreeObjectsCounter,  typeof(T).Name, mutationVariant);

                if (_currentMethod != null && _currentMethod.ContainingTypeDefinition is INamedTypeDefinition)
                {
                    mutationTarget.Method = new MethodIdentifier(_currentMethod);
                }

                
                targets.Add( mutationTarget);
            }
            string groupname = _formatter.Format(obj);
            _mutationTargets.Add(Tuple.Create(groupname, targets));
        }


        public virtual void PostProcess()
        {
            foreach (var mutationTarget in _mutationTargets.Select(t => t.Item2).Flatten())
            {
                mutationTarget.Variant.AstObjects = mutationTarget.Variant.AstObjects
                    .ToDictionary(pair => pair.Key, pair => Unspecialize(pair.Value));

                if (mutationTarget.Variant.AstObjects.Values.Any(v => !AllAstIndices.ContainsKey(v)))
                {
                    Debugger.Break();
                }
                mutationTarget.VariantObjectsIndices = mutationTarget.Variant.AstObjects.MapValues((key, val) => AllAstIndices[val]);

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

            var mutationTarget = new MutationTarget(o.GetType().Name, TreeObjectsCounter, "", 
                new MutationVariant("", new Dictionary<string, object>()));

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
        public List<Tuple<string/*GroupName*/, List<MutationTarget>>> MutationTargets
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