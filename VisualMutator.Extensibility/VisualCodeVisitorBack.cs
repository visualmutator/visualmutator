namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using System.Linq;

    public class VisualCodeVisitorBack : VisualCodeVisitor
    {
        private readonly ICollection<MutationTarget> _mutationTargets;
        private readonly List<object> _mutationTargetsElements;

        public List<object> MutationTargetsElements
        {
            get { return _mutationTargetsElements; }
        }

        public VisualCodeVisitorBack(ICollection<MutationTarget> mutationTargets)
            : base(new OperatorCodeVisitor())
        {
            _mutationTargets = mutationTargets;
            _mutationTargetsElements = new List<object>();
        }


        protected override bool Process(object obj)
        {
            base.Process(obj);
            if (_mutationTargets.Any(t => t.CounterValue == elementCounter))
            {
                _mutationTargetsElements.Add(obj);
            }
            return false;
        }



    }
}