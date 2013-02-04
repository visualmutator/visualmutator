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


    public class ChangeAdditionIntoSubstraction : IMutationOperator
    {
        public class MyVisitor : OperatorCodeVisitor
        {
            public override void Visit(IAddition addition)
            {
                if (addition.RightOperand is CompileTimeConstant)
                {
                    MarkMutationTarget(addition);
                }
            }
        }
        public class MyRewriter : OperatorCodeRewriter
        {
            public override IExpression Rewrite(IAddition addition)
            {
                return new Subtraction
                {
                    LeftOperand = addition.LeftOperand,
                    RightOperand = addition.RightOperand,
                };

            }
        }

        public string Identificator
        {
            get
            {
                return "CAIS";
            }
        }

        public string Name
        {
            get
            {
                return "Change Addition Into Substraction";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces every occurence of addition with substaction.";
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
