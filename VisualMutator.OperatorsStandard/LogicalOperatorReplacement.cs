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


    public class LogicalOperatorReplacement : IMutationOperator
    {
        public class LogicalOperatorReplacementVisitor : OperatorCodeVisitor
        {
            private void ProcessOperation(IBinaryOperation operation)
            {
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
        public class LogicalOperatorReplacementRewriter : OperatorCodeRewriter
        {
           
            private IExpression ReplaceOperation<T>(T operation) where T : IBinaryOperation
            {
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

        public string Identificator
        {
            get
            {
                return "LOR";
            }
        }

        public string Name
        {
            get
            {
                return "Logical Operator Replacement";
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
            return new LogicalOperatorReplacementVisitor();
        }

        public IOperatorCodeRewriter Mutate()
        {
            return new LogicalOperatorReplacementRewriter();
        }
    }
}
