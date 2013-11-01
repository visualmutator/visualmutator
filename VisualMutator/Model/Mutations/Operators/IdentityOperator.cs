namespace VisualMutator.Model.Mutations.Operators
{
    #region

    using Extensibility;

    #endregion

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