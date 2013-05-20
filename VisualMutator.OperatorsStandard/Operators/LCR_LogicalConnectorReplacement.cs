using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorsStandard
{
    using System.Collections;
    using System.ComponentModel.Composition;
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.FunctionalUtils;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
  

    using VisualMutator.Extensibility;


    public class LCR_LogicalConnectorReplacement : IMutationOperator
    {
        public class LCRVisitor : OperatorCodeVisitor
        {

            private void ProcessOperation(IConditional cond)
            {
                var passes = new List<string>();
                var boundCondition = cond.Condition as BoundExpression;
                
                if (boundCondition != null && boundCondition.Type.TypeCode == PrimitiveTypeCode.Boolean)
                {
                    var resultTrueBound = cond.ResultIfTrue as BoundExpression;
                    var resultFalseBound = cond.ResultIfFalse as BoundExpression;
                    var resultTrueConstant = cond.ResultIfTrue as CompileTimeConstant;
                    var resultFalseConstant = cond.ResultIfFalse as CompileTimeConstant;

                    if (resultTrueBound != null && resultTrueBound.Type.TypeCode == PrimitiveTypeCode.Boolean
                        && resultFalseConstant != null) // is &&
                    {
                        passes.Add("||");
                    }
                    else if (resultTrueConstant != null && resultFalseBound != null 
                        && resultFalseBound.Type.TypeCode == PrimitiveTypeCode.Boolean) // is ||
                    {
                        passes.Add("&&");
                    }
                }

                if (passes.Count != 0)
                {
                    MarkMutationTarget(passes);
                }
            }
        }
        public class LCRRewriter : OperatorCodeRewriter
        {
           
            private IExpression ReplaceOperation<T>(T operation) where T : IBinaryOperation
            {
                var replacement = Switch.Into<Expression>()
                    .From(MutationTarget.PassInfo)
                    .Case("LessThan", new LessThan())
                    .Case("GreaterThan", new GreaterThan())
                    .Case("GreaterThanOrEqual", new GreaterThanOrEqual())
                    .Case("LessThanOrEqual", new LessThanOrEqual())
                    .Case("Equality", new Equality())
                    .Case("NotEquality", new NotEquality())
                    .Case("True", new CompileTimeConstant{Value = true})
                    .Case("False", new CompileTimeConstant{Value = false})
                    .GetResult();
                replacement.Type = Host.PlatformType.SystemBoolean;
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
        }
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("ROR", "Relational Operator Replacement", "");
            }
        }
      

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new LCRVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new LCRRewriter();
        }
    }
}
