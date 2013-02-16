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
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using VisualMutator.Extensibility;


    public class AbsoluteValueInsertion : IMutationOperator
    {
        public class AbsoluteValueInsertionVisitor : OperatorCodeVisitor
        {
            private void ProcessOperation(IBinaryOperation operation)
            {
                var passes = new List<string>
                {
                    "Abs",
                    "NegAbs",
                }.ToList();
      
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
        public class AbsoluteValueInsertionRewriter : OperatorCodeRewriter
        {
           
            private IExpression ReplaceOperation<T>(T operation) where T : IBinaryOperation
            {
               if(MutationTarget.PassInfo == "Abs")
               {
                   
               }
                Expression result;
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
                    result = replacement;
                }
                else
                {
                    if(MutationTarget.PassInfo == "LeftParam")
                    {
                      //  BoundExpression replacement = new BoundExpression();
                     //   replacement.Definition = operation.LeftOperand;
                        //  replacement.
                        result = (Expression)operation.LeftOperand;
                    }
                    else// if (MutationTarget.PassInfo == "RightParam")
                    {
                        //  BoundExpression replacement = new BoundExpression();
                        //   replacement.Definition = operation.LeftOperand;
                        //  replacement.
                        result = (Expression)operation.RightOperand;
                    }
                }
                result.Locations = operation.Locations.ToList();
                result.Type = operation.Type;
                return result;
            }
            public override IExpression Rewrite(IAddition operation)
            {
                return ReplaceOperation(operation);
            }
            
        }

        public string Identificator
        {
            get
            {
                return "AOR";
            }
        }

        public string Name
        {
            get
            {
                return "Arithmetic Operator Replacement";
            }
        }

        public string Description
        {
            get
            {
                return "";
            }
        }

        public OperatorCodeVisitor FindTargets()
        {
            return new AbsoluteValueInsertionVisitor();
        }

        public OperatorCodeRewriter Mutate()
        {
            return new AbsoluteValueInsertionRewriter();
        }
    }
}
