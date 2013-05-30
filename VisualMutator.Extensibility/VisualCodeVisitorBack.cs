namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VisualCodeVisitorBack : VisualCodeVisitor
    {
        private readonly ICollection<MutationTarget> _mutationTargets;
        private readonly List<MutationTarget> _sharedTargets;
        private readonly List<Tuple<object,string>> _targetAstObjects;
        private readonly List<object> _sharedAstObjects;

        public List<object> SharedAstObjects
        {
            get { return _sharedAstObjects; }
        }

        public List<Tuple<object, string>> TargetAstObjects
        {
            get { return _targetAstObjects; }
        }

        public VisualCodeVisitorBack(ICollection<MutationTarget> mutationTargets, List<MutationTarget> sharedTargets)
            : base(new OperatorCodeVisitor())
        {
            _mutationTargets = mutationTargets;
            _sharedTargets = sharedTargets;
            _targetAstObjects = new List<Tuple<object, string>>();
            _sharedAstObjects = new List<object>();
        }


        protected override bool Process(object obj)
        {
            base.Process(obj);
            var target = _mutationTargets.FirstOrDefault(t => t.CounterValue == TreeObjectsCounter);
            if (target != null)
            {
                _targetAstObjects.Add(Tuple.Create(obj, target.CallTypeName));
            }
            if (_sharedTargets.Any(t => t.CounterValue == TreeObjectsCounter))
            {
                _sharedAstObjects.Add(obj);
            }
            return false;
        }



    }
}