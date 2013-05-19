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

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new OperatorCodeVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new OperatorCodeRewriter();
        }

        #endregion
    }
}