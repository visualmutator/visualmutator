namespace VisualMutator.OperatorsStandard.Operators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;

    public class OODL_OperatorDeletion : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public class AODLVisitor : OperatorCodeVisitor
        {
            private void ProcessOperation<T>(T operation) where T : IBinaryOperation
            {
                var passes = new List<string> {
                "LeftOperandRemoved",
                "RightOperandRemoved"
                };

                MarkMutationTarget(operation, passes);
                
            }
            public override void Visit(IAddition operation) //+
            {
                ProcessOperation(operation);
            }
            public override void Visit(ISubtraction operation) //-
            {
                ProcessOperation(operation);
            }
            public override void Visit(IMultiplication operation) //*
            {
                ProcessOperation(operation);
            }
            public override void Visit(IDivision operation) // /
            {
                ProcessOperation(operation);
            }
            public override void Visit(IModulus operation) // %
            {
                ProcessOperation(operation);
            }
             public override void Visit(IConditional cond) 
            {
                ProcessOperation(cond);
            }
            private void ProcessOperation (IConditional operation)
            {
                MarkMutationTarget(operation);
            }
            public override void Visit(IBitwiseAnd operation) // &
            {
                ProcessOperation(operation);
            }
            public override void Visit(IBitwiseOr operation) // |
            {
                ProcessOperation(operation);
            }
            public override void Visit(IExclusiveOr operation) // ^
            {
                ProcessOperation(operation);
            }
            public override void Visit(ILessThan operation) // <
            {
                ProcessOperation(operation);
            }
            public override void Visit(IGreaterThan operation) // >
            {
                ProcessOperation(operation);
            }
            public override void Visit(IGreaterThanOrEqual operation) // <=
            {
                ProcessOperation(operation);
            }
            public override void Visit(ILessThanOrEqual operation) // <=
            {
                ProcessOperation(operation);
            }
            public override void Visit(IEquality operation) //==
            {
                ProcessOperation(operation);
            }
            public override void Visit(INotEquality operation) // !=
            {
                ProcessOperation(operation);
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
        public class AODLRewriter : OperatorCodeRewriter
        {

            private IExpression ReplaceOperation<T>(T operation) where T : IBinaryOperation
            {
                _log.Info("Rewriting: " + operation);
                IExpression result;
                result = operation;
                
                if (MutationTarget.PassInfo.IsIn("LeftOperandRemoved"))
                {
                    try
                    {
                        BinaryOperation replacement = (BinaryOperation)operation.LeftOperand;
                        replacement.RightOperand = operation.RightOperand;
                        result = replacement;
                    }
                    catch
                    {
                        result = operation.RightOperand;
                    }
                }
                if (MutationTarget.PassInfo.IsIn("RightOperandRemoved"))
                {
                    try
                    {
                        BinaryOperation replacement = (BinaryOperation)operation.RightOperand;
                        replacement.LeftOperand = operation.LeftOperand;
                        result = replacement;
                    }
                    catch
                    {
                        result = operation.LeftOperand;
                    }
                }

                return result;
            }
            public override IExpression Rewrite(IConditional cond)
            {
                IExpression result;
                result = cond.Condition;
                return result;
            }

            public override IExpression Rewrite(IAddition operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(ISubtraction operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IMultiplication operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IDivision operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IModulus operation)
            {
                return ReplaceOperation(operation);
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
            public override IExpression Rewrite(ILessThan operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IGreaterThan operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(ILessThanOrEqual operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IGreaterThanOrEqual operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(IEquality operation)
            {
                return ReplaceOperation(operation);
            }
            public override IExpression Rewrite(INotEquality operation)
            {
                return ReplaceOperation(operation);
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
                return new OperatorInfo("OODL", "Operator Deletion", "");
            }
        }


        public IOperatorCodeVisitor CreateVisitor()
        {
            return new AODLVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new AODLRewriter();
        }
    }
}
