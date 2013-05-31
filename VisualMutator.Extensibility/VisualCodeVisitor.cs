namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using System;

    public class VisualCodeVisitor : VisualCodeVisitorBase
    {

        protected int TreeObjectsCounter { get; set; }
        private object _currentObj;
        protected readonly IDictionary<object, int> AllAstIndices;
        protected readonly IDictionary<int, object> AllAstObjects; 
        private readonly List<Tuple<string/*GroupName*/, List<MutationTarget>>> _mutationTargets;
        private readonly List<MutationTarget> _sharedTargets;
        private IMethodDefinition _currentMethod;


        public VisualCodeVisitor(IOperatorCodeVisitor visitor):base(visitor)
        {
            visitor.Parent = this;
               _mutationTargets = new List<Tuple<string/*GroupName*/, List<MutationTarget>>>();
            _sharedTargets = new List<MutationTarget>();
            AllAstIndices = new Dictionary<object, int>();
            AllAstObjects = new Dictionary<int, object>();
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

            return true;
        }

        public void MarkMutationTarget<T>(T obj, IList<MutationVariant> variants )
        {
            if (!ReferenceEquals(_currentObj, obj))
            {
                throw new ArgumentException("MarkMutationTarget must be called on current Visit method argument");
            }

            var targets = new List<MutationTarget>();
            foreach (var mutationVariant in variants)
            {
                var mutationTarget = new MutationTarget(obj.GetType().Name, 
                    TreeObjectsCounter,  typeof(T).Name, mutationVariant);

                if (_currentMethod != null && _currentMethod.ContainingTypeDefinition is INamedTypeDefinition)
                {
                    mutationTarget.Method = new MethodIdentifier(_currentMethod);
                }

                
              


                targets.Add( mutationTarget);
            }
            _mutationTargets.Add(Tuple.Create(obj.GetType().Name, targets));
        }
        public virtual void PostProcess()
        {
            foreach (var mutationTarget in _mutationTargets.Select(t => t.Item2).Flatten())
            {
                mutationTarget.VariantObjectsIndices = mutationTarget.Variant.AstObjects.MapValues((key, val) => AllAstIndices[val]);

            }
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