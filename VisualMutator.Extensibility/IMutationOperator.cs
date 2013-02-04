

namespace VisualMutator.Extensibility
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    public interface IMutationOperator
    {
     
        string Identificator { get; }
        string Name { get; }

        string Description { get; }

   
        OperatorCodeVisitor FindTargets();

        OperatorCodeRewriter Mutate();
    }
}
