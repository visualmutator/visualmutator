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


    public class SwapLogicalEquality : IMutationOperator
    {
        public class MyVisitor : OperatorCodeVisitor
        {
            public override void Visit(IEquality equality)
            {

                MarkMutationTarget(equality);
                
            }
            public override void Visit(INotEquality notEquality)
            {

                MarkMutationTarget(notEquality);

            }
        }
        public class MyRewriter : OperatorCodeRewriter
        {
            public override IExpression Rewrite(IEquality equality)
            {
                return new NotEquality
                {
                    LeftOperand = equality.LeftOperand,
                    RightOperand = equality.RightOperand,
                };

            }
            public override IExpression Rewrite(INotEquality notEquality)
            {
                return new Equality
                {
                    LeftOperand = notEquality.LeftOperand,
                    RightOperand = notEquality.RightOperand,
                };

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
            return new MyVisitor();
        }

        public OperatorCodeRewriter Mutate()
        {
            return new MyRewriter();
        }
    }
}
