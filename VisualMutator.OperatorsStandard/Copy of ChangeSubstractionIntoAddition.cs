using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorsStandard
{
    using System.Collections;
    using System.ComponentModel.Composition;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using VisualMutator.Extensibility;


    public class ArithmeticOperatorInsertion : IMutationOperator
    {
        public class ArithmeticOperatorInsertionVisitor : OperatorCodeVisitor
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
                
                if(operation.LeftOperand is BoundExpression)
                {
                    passes.Add("LeftParam");
                }
                if (operation.RightOperand is BoundExpression)
                {
                    passes.Add("RightParam");
                }
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
        public class ArithmeticOperatorInsertionRewriter : OperatorCodeRewriter
        {
            private List<Type> input = new List<Type>
            {
                typeof(Addition),
                typeof(Subtraction),
                typeof(Multiplication),
                typeof(Division),
                typeof(Modulus),

            };
            private List<Type> output = new List<Type>
            {
                typeof(Addition),
                typeof(Subtraction),
                typeof(Multiplication),
                typeof(Division),
                typeof(Modulus),
                typeof(BoundExpression),
                typeof(BoundExpression),
            };
            private IExpression ReplaceOperation<T>(T operation) where T : IBinaryOperation
            {
                input.RemoveAll(delegate(Type t)
                {
                    return t.GetInterfaces().Contains(operation.GetType());

                });
                Expression result;
                if(MutationTarget.CurrentPass <= 3)
                {
                    Type t = Type.GetType("Microsoft.Cci." + MutationTarget.PassInfo);
                    BinaryOperation replacement = (BinaryOperation)Activator.CreateInstance(t);
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
                return "SLE";
            }
        }

        public string Name
        {
            get
            {
                return "Swap Logical Equality";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces every occurence of equality with not equality and vice versa.";
            }
        }

        public OperatorCodeVisitor FindTargets()
        {
            return new ArithmeticOperatorInsertionVisitor();
        }

        public OperatorCodeRewriter Mutate()
        {
            return new ArithmeticOperatorInsertionRewriter();
        }
    }
}
