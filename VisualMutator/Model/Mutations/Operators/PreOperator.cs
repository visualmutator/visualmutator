namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;

    using Mono.Cecil;

    using VisualMutator.Extensibility;

    class PreOperator : IMutationOperator
    {
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

        public IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types)
        {
            yield return new MutationTarget();
        }

        public void Mutate(MutationTarget target, IList<AssemblyDefinition> assembliesToMutate)
        {
            
        }

    }
}