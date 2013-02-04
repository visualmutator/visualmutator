namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;

    using Mono.Cecil;

    using VisualMutator.Extensibility;

    class PreOperator : IMutationOperator
    {
        public string Identificator
        {
            get
            {
                return "PRE";
            }
        }

        public string Name
        {
            get
            {
                return "PreOperator";
            }
        }

        public string Description
        {
            get
            {
                return "PreOperator";
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