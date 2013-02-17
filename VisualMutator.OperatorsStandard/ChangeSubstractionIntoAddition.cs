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
  

    using VisualMutator.Extensibility;


    public class ChangeSubstractionIntoAddition : IMutationOperator
    {
        public class MyVisitor : OperatorCodeVisitor
        {
            public override void Visit(ISubtraction subtraction)
            {

                MarkMutationTarget(subtraction);
                
            }
        }
        public class MyRewriter : OperatorCodeRewriter
        {
            public override IExpression Rewrite(ISubtraction subtraction)
            {
                return new Addition
                {
                    LeftOperand = subtraction.LeftOperand,
                    RightOperand = subtraction.RightOperand,
                };

            }
        }

        public string Identificator
        {
            get
            {
                return "CSIA";
            }
        }

        public string Name
        {
            get
            {
                return "Change Substraction Into Addition";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces every occurence of substaction with addition.";
            }
        }

        public OperatorCodeVisitor FindTargets()
        {
            return new MyVisitor();
        }

        public OperatorCodeRewriter Mutate()
        {
            return new MyRewriter();
        }
    }
}
