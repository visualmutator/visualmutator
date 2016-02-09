namespace VisualMutator.OperatorsObject.Operators.Variables
{
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;

    public class RSO_RemoveStatementOperator : IMutationOperator
    {
        public class RSOVisitor : OperatorCodeVisitor
        {
            public override void Visit(IExpressionStatement statement)
            {
               var assignment = statement.Expression as IAssignment;
               if (assignment != null && !Parent.CurrentMethod.IsConstructor)
               {
                    MarkMutationTarget(statement);
               }
            }

        }
        public class RSORewriter : OperatorCodeRewriter
        {
            public override IStatement Rewrite(IExpressionStatement statement)
            {
                return new EmptyStatement();
            }
        }
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("RSO", "Remove Statement Operator", "");
            }
        }
     

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new RSOVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new RSORewriter();
        }
    }
}
