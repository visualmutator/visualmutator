namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VisualCodeVisitorBack : VisualCodeVisitor
    {
        private readonly ICollection<MutationTarget> _mutationTargets;
        private readonly List<MutationTarget> _commonTargets;
        private readonly List<Tuple<object,string>> _mutationTargetsElements;
        private readonly List<object> _commonTargetsElements;

        public List<object> CommonTargetsElements
        {
            get { return _commonTargetsElements; }
        }

        public List<Tuple<object, string>> MutationTargetsElements
        {
            get { return _mutationTargetsElements; }
        }

        public VisualCodeVisitorBack(ICollection<MutationTarget> mutationTargets, List<MutationTarget> commonTargets)
            : base(new OperatorCodeVisitor())
        {
            _mutationTargets = mutationTargets;
            _commonTargets = commonTargets;
            _mutationTargetsElements = new List<Tuple<object, string>>();
            _commonTargetsElements = new List<object>();
        }


        protected override bool Process(object obj)
        {
            base.Process(obj);
            var target = _mutationTargets.FirstOrDefault(t => t.CounterValue == elementCounter);
            if (target != null)
            {
                _mutationTargetsElements.Add(Tuple.Create(obj, target.CallTypeName));
            }
            if (_commonTargets.Any(t => t.CounterValue == elementCounter))
            {
                _commonTargetsElements.Add(obj);
            }
            return false;
        }



    }
}