namespace VisualMutator.Controllers
{
    using System.Collections.Generic;

    using CommonUtilityInfrastructure.Paths;
    using Model;
    using Mono.Cecil;

    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Types;

    public class MutationSessionChoices 
    {
        public IList<IMutationOperator> SelectedOperators
        {
            get;
            set;
        }

        public IList<AssemblyNode> Assemblies
        {
            get;
            set;
        }
        public IList<DirectoryPathAbsolute> ProjectPaths
        {
            get;
            set;
        }

        public LoadedTypes SelectedTypes
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