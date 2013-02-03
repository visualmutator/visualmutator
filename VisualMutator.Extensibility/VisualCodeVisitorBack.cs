namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    public class VisualCodeVisitorBack : VisualCodeVisitor
    {
        private readonly List<int> _mutationTargets;
        private readonly List<object> _mutationTargetsElements;

        public List<object> MutationTargetsElements
        {
            get { return _mutationTargetsElements; }
        }

        public VisualCodeVisitorBack(List<int> mutationTargets)
            : base(new OperatorCodeVisitor())
        {
            _mutationTargets = mutationTargets;
            _mutationTargetsElements = new List<object>();
        }


        protected override bool Process(object obj)
        {
            base.Process(obj);
            if (_mutationTargets.Contains(elementCounter))
            {
                _mutationTargetsElements.Add(obj);
            }
            return false;
        }



    }
}