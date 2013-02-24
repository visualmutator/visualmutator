namespace VisualMutator.Tests.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Extensibility;

    public class TestOperator : IMutationOperator
    {
        public OperatorInfo Info { get{return new OperatorInfo("Test", "", "");} }
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