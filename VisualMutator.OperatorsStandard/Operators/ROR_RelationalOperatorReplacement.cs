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


    public class ROR_RelationalOperatorReplacement : IMutationOperator
    {
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("ROR", "Relational operator replacement", "");
            }
        }
        public class RORVisitor : OperatorCodeVisitor
        {
            
      
            private void ProcessOperation<T>(T operation) where T : IBinaryOperation
            {
                var operandTypeCode = operation.LeftOperand.Type.TypeCode;
                var passes = new List<string>
                    {
                        "True",
                        "False",
                    };

                //ALL: true, false
                //integer: all
                // float less, greater
                // bool, object: equals, ne


                if (operandTypeCode.IsIn(PrimitiveTypeCode.Boolean, 
                    PrimitiveTypeCode.NotPrimitive, PrimitiveTypeCode.Char, 
                    PrimitiveTypeCode.Reference, PrimitiveTypeCode.String))
                {
                    passes.AddRange("Equality", "NotEquality");
                }
                else
                {
                    passes.AddRange("LessThan", "GreaterThan");
                    passes.AddRange("LessThanOrEqual", "GreaterThanOrEqual");
                    passes.AddRange("Equality", "NotEquality");
                }
                passes = passes.Where(elem => elem != operation.GetType().Name).ToList();
             

                MarkMutationTarget(operation, passes);
            }

            public override void Visit(ILessThan operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(IGreaterThan operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(IGreaterThanOrEqual operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(ILessThanOrEqual operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(IEquality operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(INotEquality operation)
            {
                ProcessOperation(operation);
            }
        }
        public class RORRewriter : OperatorCodeRewriter
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
        

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new RORVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new RORRewriter();
        }
    }
}
