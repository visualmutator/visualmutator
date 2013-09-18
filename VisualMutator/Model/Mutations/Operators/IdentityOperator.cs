namespace VisualMutator.Model.Mutations.Operators
{
    using VisualMutator.Extensibility;

    class IdentityOperator : IMutationOperator
    {
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("I", "Identity operator", "Operator that does not create any changes.");
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