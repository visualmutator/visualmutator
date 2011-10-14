namespace VisualMutator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using VisualMutator.Model.Tests.TestsTree;

    public class ExecutedOperator : MutationNode
    {
        public ExecutedOperator(MutationRootNode parent, string name)
            : base(parent, name, true)
        {
        }

        public IEnumerable<Mutant> Mutants
        {
            get
            {
                return Children.Cast<Mutant>();
            }
        }
    }
}