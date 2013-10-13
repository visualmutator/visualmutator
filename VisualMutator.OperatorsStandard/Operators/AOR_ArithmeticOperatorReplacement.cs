using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorsStandard
{
    using System.Collections;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.FunctionalUtils;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
  

    using VisualMutator.Extensibility;
    using log4net;


    public class AOR_ArithmeticOperatorReplacement : IMutationOperator
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public class AORVisitor : OperatorCodeVisitor
        {
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
            private void ProcessOperation<T>(T operation) where T : IBinaryOperation
            {
                _log.Info("Visiting: " + operation);
                var passes = new List<string>
                {
                    "Addition",
                    "Subtraction",
                    "Multiplication",
                    "Division",
                    "Modulus",
                }.Where(elem => elem != operation.GetType().Name).ToList();

                if (!operation.ResultIsUnmodifiedLeftOperand && !(operation.LeftOperand is ITargetExpression))
                {
                    passes.Add("LeftParam");
                    passes.Add("RightParam");
                }
          
                MarkMutationTarget(operation, passes);
            }

        }
        public class AORRewriter : OperatorCodeRewriter
        {
           
            private IExpression ReplaceOperation<T>(T operation) where T : IBinaryOperation
            {
                _log.Info("Rewriting: " + operation);
                IExpression result;
                if (!MutationTarget.PassInfo.IsIn("LeftParam", "RightParam"))
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
                    result = Switch.Into<IExpression>()
                        .From(MutationTarget.PassInfo)
                        .Case("LeftParam", operation.LeftOperand)
                        .Case("RightParam", operation.RightOperand)
                        .GetResult();
                   
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
     

        public IOperatorCodeVisitor CreateVisitor()
        {
            return new AORVisitor();
        }

        public IOperatorCodeRewriter CreateRewriter()
        {
            return new AORRewriter();
        }
    }
}
