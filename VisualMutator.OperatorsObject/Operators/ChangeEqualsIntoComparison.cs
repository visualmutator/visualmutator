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

   // using OpCodes = Mono.Cecil.Cil.OpCodes;

    public class ChangeEqualsIntoComparison : IMutationOperator
    {


        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("CEIC", "Change Equals Into Comparison", "");
            }
        }
      

        public IOperatorCodeVisitor FindTargets()
        {
            return new OperatorCodeVisitor();

        }

        public IOperatorCodeRewriter Mutate()
        {
            return new OperatorCodeRewriter();
        }
    }
}
