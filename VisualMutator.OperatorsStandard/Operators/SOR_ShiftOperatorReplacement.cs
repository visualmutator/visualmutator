namespace VisualMutator.OperatorsStandard.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.Switches;

    public class SOR_ShiftOperatorReplacement : IMutationOperator
    {
        public class ShiftOperatorReplacementVisitor : OperatorCodeVisitor
        {
            private void ProcessOperation<T>(T operation) where T : IBinaryOperation
            {
                var passes = new List<string>
                    {
                        "RightShift",
                        "LeftShift", 
                    }.Where(elem => elem != operation.GetType().Name).ToList();
                
                MarkMutationTarget(operation, passes);
            }
            public override void Visit(IRightShift operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(ILeftShift operation)
            {
                ProcessOperation(operation);
            }
         
        }
        public class ShiftOperatorReplacementRewriter : OperatorCodeRewriter
        {
           
            private IExpression ReplaceOperation<T>(T operation) where T : IBinaryOperation
            {
                var replacement = Switch.Into<Expression>()
                    .From(MutationTarget.PassInfo)
                    .Case("RightShift", new RightShift())
                    .Case("LeftShift", new LeftShift())
                    .GetResult();

                replacement.Type = operation.Type;
                var binary = replacement as BinaryOperation;
                if (binary != null)
                {
                    binary.LeftOperand = operation.LeftOperand;
                    binary.RightOperand = operation.RightOperand;
                    binary.ResultIsUnmodifiedLeftOperand = operation.ResultIsUnmodifiedLeftOperand;
                }

                replacement.Locations = operation.Locations.ToList();

                return replacement;
            }
            public override IExpression Rewrite(IRightShift operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(ILeftShift operation)
            {
                return ReplaceOperation(operation);
            }
            
        }
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("SOR", "Shift Operator Replacement", "");
            }
        }


        public IOperatorCodeVisitor CreateVisitor()
        {
            return new ShiftOperatorReplacementVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new ShiftOperatorReplacementRewriter();
        }
    }
}
