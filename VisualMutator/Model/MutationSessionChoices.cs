namespace VisualMutator.Model
{
    #region

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Extensibility;
    using Microsoft.Cci;
    using Mutations.Types;
    using NUnit.Framework.Constraints;
    using Tests;
    using UsefulTools.Paths;

    #endregion

    public class MutationSessionChoices
    {
        public MutationSessionChoices()
        {
        SelectedOperators = new List<IMutationOperator>();
            Assemblies = new List<AssemblyNode>();
            ProjectPaths = new List<DirectoryPathAbsolute>();
            SelectedTypes = new LoadedTypes(new List<INamespaceTypeDefinition>());
            MutantsTestingOptions= new MutantsTestingOptions();
            MutantsCreationOptions= new MutantsCreationOptions();
            SelectedTests = new Collection<TestId>();
        }

        public IList<IMutationOperator> SelectedOperators { get; set; }
        public IList<AssemblyNode> Assemblies { get; set; }
        public IList<DirectoryPathAbsolute> ProjectPaths { get; set; }
        public LoadedTypes SelectedTypes { get; set; }
        //only valid when creating mutants on disk
        public string MutantsCreationFolderPath { get; set; }
        public MutantsTestingOptions MutantsTestingOptions { get; set; }
        public MutantsCreationOptions MutantsCreationOptions { get; set; }
        public ICollection<TestId> SelectedTests { get; set; }
        public List<string> AssembliesPaths { get; set; }
    }
}