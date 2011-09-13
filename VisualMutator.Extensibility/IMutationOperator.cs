

namespace VisualMutator.Extensibility
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;
    public class MutationResultDetails
        {
            public ICollection<MethodDefinition> ModifiedMethods
            {
                get;
                set;
            }

        public IMutationOperator Operator { get; set; }
        }

    public interface IMutationOperator
    {
        MutationResultDetails Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types);

        string Name { get; }

        string Description { get; }


    }
}
