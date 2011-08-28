

namespace VisualMutator.Extensibility
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    public interface IMutationOperator
    {
        void Mutate(IEnumerable<TypeDefinition> assemblyPath);

        string Name { get; }

        string Description { get; }


    }
}
