namespace VisualMutator.Controllers
{
    using System.Collections.Generic;

    using Mono.Cecil;

    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations;

 
    public class MutationSessionChoices 
    {
        public IList<IMutationOperator> SelectedOperators
        {
            get;
            set;
        }

        public IList<AssemblyDefinition> Assemblies
        {
            get;
            set;
        }

        public IList<TypeDefinition> SelectedTypes
        {
            get;
            set;
        }
        public string MutantsCreationFolderPath
        {
            get;
            set;
        }

        
     
        public MutantsTestingOptions MutantsTestingOptions
        {
            get;
            set;
        }
        public MutantsCreationOptions MutantsCreationOptions
        {
            get;
            set;
        }
    }
}