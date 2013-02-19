namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;

    using Mono.Cecil;

    using VisualMutator.Extensibility;

    class PreOperator : IMutationOperator
    {
        public OperatorInfo Info
        {
            get
            {
                return new OperatorInfo("PRE", "PreOperator", "");
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