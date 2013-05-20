namespace VisualMutator.OperatorsObject.Operators
{
    using System.Linq;

    using Microsoft.Cci.MutableCodeModel;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;


    public class JTD_ThisKeywordDeletion : IMutationOperator
    {
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("JTD", "This keyword deletion", "");
            }
        }
      
        public class JTDVisitor : OperatorCodeVisitor
        {
            public override void Visit(IExpressionStatement statement)
            {
                var assignment = statement.Expression as IAssignment;
                if (assignment != null && Parent.CurrentMethod.IsConstructor)
                {
                    if (assignment.Target.Definition is IFieldReference
                        && assignment.Target.Instance is ThisReference)
                    {
                        MarkMutationTarget(statement);
                    }
                }
            }
        }

        public class JTDRewriter : OperatorCodeRewriter
        {

            public override IStatement Rewrite(IExpressionStatement statement)
            {
                return new EmptyStatement();
            }
        }

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new JTDVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new JTDRewriter();
        }

    }
}