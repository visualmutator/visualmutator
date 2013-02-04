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

       

        public string Identificator
        {
            get
            {
                return "CEIC";
            }
        }

        public string Name
        {
            get
            {
                return "Change Equals Into Comparison";
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
            return new OperatorCodeVisitor();

        }

        public OperatorCodeRewriter Mutate()
        {
            return new OperatorCodeRewriter();
        }
    }
}
