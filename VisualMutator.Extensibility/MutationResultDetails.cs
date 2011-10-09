namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using Mono.Cecil;

    public class MutationResultsCollection
    {


        public IList<MutationResult> MutationResults
        {
            get;
            set;
        }
    }


    public class MutationResult
    {


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