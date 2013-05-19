namespace VisualMutator.Model.Mutations.Operators
{
    using VisualMutator.Extensibility;

    class PreOperator : IMutationOperator
    {
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("PRE", "PreOperator", "");
            }
        }
      
        public IOperatorCodeVisitor CreateVisitor()
        {
            return new OperatorCodeVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new OperatorCodeRewriter();
        }
    }
}