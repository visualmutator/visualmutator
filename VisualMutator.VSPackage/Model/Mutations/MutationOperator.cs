namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using VisualMutator.Extensibility;

    public class MutationOperator : TreeElement
    {
        public IMutationOperator Operator { get; set; }

        public MutationOperator(IMutationOperator mutationOperator)
        {
            Operator = mutationOperator;
            Name = "oper";
        }

        
    }
}