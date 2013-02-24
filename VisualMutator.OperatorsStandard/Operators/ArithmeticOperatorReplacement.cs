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


    public class ArithmeticOperatorReplacement : IMutationOperator
    {
        public class ArithmeticOperatorReplacementVisitor : OperatorCodeVisitor
        {
            private void ProcessOperation(IBinaryOperation operation)
            {
                var passes = new List<string>
                {
                    "Addition",
                    "Subtraction",
                    "Multiplication",
                    "Division",
                    "Modulus",
                }.Where(elem => elem != operation.GetType().Name).ToList();

               // if (operation.LeftOperand.IsAnyOf<BoundExpression, CompileTimeConstant>())
              //  {
                    passes.Add("LeftParam");
              //  }
              //  if (operation.RightOperand.IsAnyOf<BoundExpression, CompileTimeConstant>())
           //     {
                    passes.Add("RightParam");
          //      }
                MarkMutationTarget(operation, passes);
            }

            public override void Visit(IAddition operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(ISubtraction operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(IMultiplication operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(IDivision operation)
            {
                ProcessOperation(operation);
            }
            public override void Visit(IModulus operation)
            {
                ProcessOperation(operation);
            }
        }
        public class ArithmeticOperatorReplacementRewriter : OperatorCodeRewriter
        {
           
            private IExpression ReplaceOperation<T>(T operation) where T : IBinaryOperation
            {
               
                IExpression result;
                if(MutationTarget.CurrentPass <= 3)
                {
                   var replacement = Switch.Into<BinaryOperation>()
                        .From(MutationTarget.PassInfo)
                        .Case("Addition", new Addition())
                        .Case("Subtraction", new Subtraction())
                        .Case("Multiplication", new Multiplication())
                        .Case("Division", new Division())
                        .Case("Modulus", new Modulus())
                        .GetResult();
                   
                    replacement.LeftOperand = operation.LeftOperand;
                    replacement.RightOperand = operation.RightOperand;
                    replacement.ResultIsUnmodifiedLeftOperand = operation.ResultIsUnmodifiedLeftOperand;
                    replacement.Type = operation.Type;
                    result = replacement;
                }
                else
                {
                    if(MutationTarget.PassInfo == "LeftParam")
                    {
                        result = operation.LeftOperand;
                    }
                    else// if (MutationTarget.PassInfo == "RightParam")
                    {
 
                        result = operation.RightOperand;
                    }
                }
                //result.Locations = operation.Locations.ToList();
                
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
        }
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("AOR", "Arithmetic Operator Replacement", "");
            }
        }
     

        public IOperatorCodeVisitor FindTargets()
        {
            return new ArithmeticOperatorReplacementVisitor();
        }

        public IOperatorCodeRewriter Mutate()
        {
            return new ArithmeticOperatorReplacementRewriter();
        }
    }
}
