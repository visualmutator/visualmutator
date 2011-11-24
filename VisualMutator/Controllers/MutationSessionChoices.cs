namespace VisualMutator.Controllers
{
    using System.Collections.Generic;

    using Mono.Cecil;

    using VisualMutator.Extensibility;

    public class MutationSessionChoices
    {
        public IList<IMutationOperator> SelectedOperators
        {
            get;
            set;
        }

        public IList<AssemblyDefinition> Assemblies
        {
            get; set;
        }

        public IList<TypeDefinition> SelectedTypes
        {
            get;
            set;
        }

        public IList<string> AdditionalFilesToCopy 
        { get;
            set;
        }

        public bool CreateMoreMutants { get; set; }

        public int TestingTimeoutSeconds { get; set; }
    }
}