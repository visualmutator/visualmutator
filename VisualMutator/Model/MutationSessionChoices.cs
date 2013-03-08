namespace VisualMutator.Model
{
    using System.Collections.Generic;
    using CommonUtilityInfrastructure.Paths;
    using NUnit.Core;
    using Tests;
    using VisualMutator.Extensibility;
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

        public ICollection<TestId> SelectedTests
        {
            get;
            set;
        }
    }
}