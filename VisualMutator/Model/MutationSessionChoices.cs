namespace VisualMutator.Model
{
    #region

    using System.Collections.Generic;
    using Extensibility;
    using Mutations.Types;
    using Tests;
    using UsefulTools.Paths;

    #endregion

    public class MutationSessionChoices
    {
        public IList<IMutationOperator> SelectedOperators { get; set; }
        public IList<AssemblyNode> Assemblies { get; set; }
        public IList<DirectoryPathAbsolute> ProjectPaths { get; set; }
        public LoadedTypes SelectedTypes { get; set; }
        //only valid when creating mutants on disk
        public string MutantsCreationFolderPath { get; set; }
        public MutantsTestingOptions MutantsTestingOptions { get; set; }
        public MutantsCreationOptions MutantsCreationOptions { get; set; }
        public ICollection<TestId> SelectedTests { get; set; }
    }
}