namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using VisualMutator.Extensibility;

    public class MutationOperator : ExtModel
    {
        public IMutationOperator Operator { get; set; }

        public MutationOperator(IMutationOperator mutationOperator)
        {
            Operator = mutationOperator;
        }

        public bool IsEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}