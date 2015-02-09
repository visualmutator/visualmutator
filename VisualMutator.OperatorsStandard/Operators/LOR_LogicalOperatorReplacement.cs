namespace VisualMutator.OperatorsStandard.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.Switches;

    public class LOR_LogicalOperatorReplacement : IMutationOperator
    {

        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public class LORVisitor : OperatorCodeVisitor
        {
            private void ProcessOperation<T>(T operation) where T : IBinaryOperation
            {
             //  _log.Info("Visiting: " + operation);
                
                var passes = new List<string>
                    {
                        "BitwiseAnd",
                        "BitwiseOr", 
                        "ExclusiveOr",
                        "OnesComplementLeft",
                        "OnesComplementRight",
                    }.Where(elem => elem != operation.GetType().Name).ToList();
                
                MarkMutationTarget(operation, passes);
            }

            public override void Visit(IBitwiseAnd operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(IBitwiseOr operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(IExclusiveOr operation)
            {
                ProcessOperation(operation);
            }
         
        }
        public class LORRewriter : OperatorCodeRewriter
        {
           
            private IExpression ReplaceOperation<T>(T operation) where T : IBinaryOperation
            {
                _log.Info("Rewriting: " + operation + " Pass: " + MutationTarget.PassInfo);
                
                var replacement = Switch.Into<Expression>()
                    .From(MutationTarget.PassInfo)
                    .Case("BitwiseAnd", new BitwiseAnd())
                    .Case("BitwiseOr", new BitwiseOr())
                    .Case("ExclusiveOr", new ExclusiveOr())
                    .Case("OnesComplementLeft", new OnesComplement{Operand = operation.LeftOperand})
                    .Case("OnesComplementRight", new OnesComplement{Operand = operation.RightOperand})
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
            public override IExpression Rewrite(IBitwiseAnd operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IBitwiseOr operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IExclusiveOr operation)
            {
                return ReplaceOperation(operation);
            }
            
        }
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("LOR", "Logical Operator Replacement", "");
            }
        }
      

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new LORVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new LORRewriter();
        }
    }
}
