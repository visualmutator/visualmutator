namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    public class VisualCodeVisitorBack : VisualCodeVisitor
    {
        private readonly ICollection<MutationTarget> _mutationTargets;
        private readonly List<MutationTarget> _commonTargets;
        private readonly List<object> _mutationTargetsElements;
        private readonly List<object> _commonTargetsElements;

        public List<object> CommonTargetsElements
        {
            get { return _commonTargetsElements; }
        }

        public List<object> MutationTargetsElements
        {
            get { return _mutationTargetsElements; }
        }

        public VisualCodeVisitorBack(ICollection<MutationTarget> mutationTargets, List<MutationTarget> commonTargets)
            : base(new OperatorCodeVisitor())
        {
            _mutationTargets = mutationTargets;
            _commonTargets = commonTargets;
            _mutationTargetsElements = new List<object>();
            _commonTargetsElements = new List<object>();
        }


        protected override bool Process(object obj)
        {
            base.Process(obj);
            if (_mutationTargets.Any(t => t.CounterValue == elementCounter))
            {
                _mutationTargetsElements.Add(obj);
            }
            if (_commonTargets.Any(t => t.CounterValue == elementCounter))
            {
                _commonTargetsElements.Add(obj);
            }
            return false;
        }



    }
}