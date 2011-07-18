

namespace VisualMutator.Extensibility
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public interface IMutationOperator
    {
        void Mutate(string assemblyPath);
    }
}
