namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using Mono.Cecil;
    using Mono.Collections.Generic;

    public class MutationResultsCollection
    {
        public MutationResultsCollection()
        {
            MutationResults = new List<MutationResult>();
        }

        public IList<MutationResult> MutationResults
        {
            get;
            set;
        }
    }


    public class MutationResult
    {
        public MutationResult(MutationTarget mutationTarget, IList<AssemblyDefinition> assembliesToMutate)
        {
            MutationTarget = mutationTarget;
            MutatedAssemblies = assembliesToMutate;
        }

        public IEnumerable<AssemblyDefinition> MutatedAssemblies
        {
            get;
            set;
        }

        public MutationTarget MutationTarget
        {
            get;
            set;
        }


    }


}