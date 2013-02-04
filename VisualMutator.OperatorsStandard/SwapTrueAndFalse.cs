using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorsStandard
{
    using System.Collections;
    using System.ComponentModel.Composition;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using VisualMutator.Extensibility;



    public class SwapTrueAndFalse : IMutationOperator
    {
        

        public string Identificator
        {
            get
            {
                return "STF";
            }
        }

        public string Name
        {
            get
            {
                return "Swap True and False";
            }
        }

        public string Description
        {
            get
            {
                return "Replaces every occurence of true and false.";
            }
        }

        public OperatorCodeVisitor FindTargets()
        {
            return new OperatorCodeVisitor();
        }

        public OperatorCodeRewriter Mutate()
        {
            return new OperatorCodeRewriter();
        }
    }
}
