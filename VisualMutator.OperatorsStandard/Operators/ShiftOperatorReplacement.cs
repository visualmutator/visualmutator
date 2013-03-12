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


    public class ShiftOperatorReplacement : IMutationOperator
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


        public IOperatorCodeVisitor FindTargets()
        {
            return new ShiftOperatorReplacementVisitor();
        }

        public IOperatorCodeRewriter Mutate()
        {
            return new ShiftOperatorReplacementRewriter();
        }
    }
}
