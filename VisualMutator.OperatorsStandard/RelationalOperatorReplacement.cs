using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorsStandard
{
    using System.Collections;
    using System.ComponentModel.Composition;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
  

    using VisualMutator.Extensibility;


    public class RelationalOperatorReplacement : IMutationOperator
    {
        public class RelationalOperatorReplacementVisitor : OperatorCodeVisitor
        {
            private void ProcessOperation(IBinaryOperation operation)
            {
                var passes = new List<string>
                    {
                        "True",
                        "False",
                    };

                if (operation.LeftOperand.Type.TypeCode != PrimitiveTypeCode.Boolean 
                    && operation.RightOperand.Type.TypeCode != PrimitiveTypeCode.Boolean)
                {
                    var passesOperandNotBool = new List<string>
                    {
                        "LessThan",
                        "GreaterThan",
                    }.Where(elem => elem != operation.GetType().Name).ToList();
                    passes.AddRange(passesOperandNotBool);
                }
                if (!operation.Type.TypeCode.IsIn(PrimitiveTypeCode.Float32, PrimitiveTypeCode.Float64))
                {
                    var passesNotFloat = new List<string>
                        {
                            "GreaterThanOrEqual",
                            "LessThanOrEqual",
                            "Equality",
                            "NotEquality",
                        }.Where(elem => elem != operation.GetType().Name);
                    passes.AddRange(passesNotFloat);
                }
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
        public class RelationalOperatorReplacementRewriter : OperatorCodeRewriter
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

        public string Identificator
        {
            get
            {
                return "ROR";
            }
        }

        public string Name
        {
            get
            {
                return "Relational Operator Replacement";
            }
        }

        public string Description
        {
            get
            {
                return "";
            }
        }

        public IOperatorCodeVisitor FindTargets()
        {
            return new RelationalOperatorReplacementVisitor();
        }

        public IOperatorCodeRewriter Mutate()
        {
            return new RelationalOperatorReplacementRewriter();
        }
    }
}
