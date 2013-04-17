namespace VisualMutator.Tests.Mutations
{
    using Extensibility;

    public class TestOperator : IMutationOperator
    {
        #region IMutationOperator Members

        public OperatorInfo Info
        {
            get { return new OperatorInfo("Test", "", ""); }
        }

        public IOperatorCodeVisitor FindTargets()
        {
            return new OperatorCodeVisitor();
        }

        public IOperatorCodeRewriter Mutate()
        {
            return new OperatorCodeRewriter();
        }

        #endregion
    }
}