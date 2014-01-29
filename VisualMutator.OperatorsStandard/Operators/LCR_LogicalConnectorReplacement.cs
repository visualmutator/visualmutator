namespace VisualMutator.OperatorsStandard.Operators
{
    using System.Collections.Generic;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using UsefulTools.Switches;

    public class LCR_LogicalConnectorReplacement : IMutationOperator
    {

        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("LCR", "Logical Connector Replacement", "");
            }
        }


        public class LCRVisitor : OperatorCodeVisitor
        {

            public override void Visit(IConditional cond)
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
                        MarkMutationTarget(cond, "to||");
                    }
                    else if (resultTrueConstant != null && resultFalseBound != null 
                        && resultFalseBound.Type.TypeCode == PrimitiveTypeCode.Boolean) // is ||
                    {
                        MarkMutationTarget(cond, "to&&");
                    }
                }

              
            }
        }
        public class LCRRewriter : OperatorCodeRewriter
        {


            public override IExpression Rewrite(IConditional cond)
            {
                var newCond = new Conditional(cond);
                Switch.On(MutationTarget.PassInfo)
                .Case("to&&", () =>
                {
                    newCond.ResultIfTrue = cond.ResultIfFalse;
                    newCond.ResultIfFalse = new CompileTimeConstant
                    {
                        Type = cond.ResultIfTrue.Type,
                        Value = false,
                    };  
                })
                .Case("to||", () =>
                {
                    newCond.ResultIfFalse = cond.ResultIfTrue;
                    newCond.ResultIfTrue = new CompileTimeConstant
                    {
                        Type = cond.ResultIfFalse.Type,
                        Value = true,
                    };          
                }).ThrowIfNoMatch();
              
                return newCond;
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
