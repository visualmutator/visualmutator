namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Cci;
    using System;

    public class VisualCodeVisitor : VisualCodeVisitorBase
    {
        
        protected int elementCounter;

        private List<Tuple<string/*GroupName*/, List<MutationTarget>>> mutationTargets;
        private List<MutationTarget> commonTargets;

        private IMethodDefinition _currentMethod;
		public IMethodDefinition CurrentMethod
        {
            get { return _currentMethod; }
        }
        public List<Tuple<string/*GroupName*/, List<MutationTarget>>> MutationTargets
        {
            get { return mutationTargets; }
        }
        public List<MutationTarget> CommonTargets
        {
            get
            {
                return commonTargets;
            }
        }
        public VisualCodeVisitor(IOperatorCodeVisitor visitor):base(visitor)
        {
            visitor.Parent = this;
               mutationTargets = new List<Tuple<string/*GroupName*/, List<MutationTarget>>>();
            commonTargets = new List<MutationTarget>();
        }

        private object _currentObj;

        protected override bool Process(object obj)
        {
            elementCounter++;
            _currentObj = obj;
            return true;
        }

        public void MarkMutationTarget<T>(T obj, List<string> passesInfo)
        {
            if (!ReferenceEquals(_currentObj, obj))
            {
                throw new ArgumentException("MarkMutationTarget must be called on current Visit method argument");
            }
            if (passesInfo == null)
            {
                passesInfo = new[]{""}.ToList();
            }
            var targets = new List<MutationTarget>();
            for (int i = 0; i < passesInfo.Count; i++)
            {
                var mutationTarget = new MutationTarget(obj.GetType().Name, elementCounter, i, passesInfo[i], typeof(T).Name);
                if (_currentMethod != null && _currentMethod.ContainingTypeDefinition is INamedTypeDefinition)
                {
                    mutationTarget.Method = new MethodIdentifier(_currentMethod);
                }

                targets.Add( mutationTarget);
            }
            mutationTargets.Add(Tuple.Create(obj.GetType().Name, targets));
        }

        public void MarkCommon<T>(T o)
        {

            var mutationTarget = new MutationTarget(o.GetType().Name, elementCounter, 0, "", "");

            commonTargets.Add(mutationTarget);


        }

        public void MethodEnter(IMethodDefinition method)
        {
            _currentMethod = method;
        }

        public void MethodExit(IMethodDefinition method)
        {
            _currentMethod = null;
        }





       
    }
}