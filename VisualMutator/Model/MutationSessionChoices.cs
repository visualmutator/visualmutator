namespace VisualMutator.Model
{
    #region

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Extensibility;
    using Microsoft.Cci;
    using Mutations;
    using Mutations.Types;
    using StoringMutants;
    using Tests;
    using Tests.TestsTree;
    using UsefulTools.Paths;

    #endregion

    public class MutationSessionChoices
    {
        public MutationSessionChoices()
        {
            SelectedOperators = new List<IMutationOperator>();
            ProjectPaths = new List<DirectoryPathAbsolute>();
            MutantsTestingOptions= new MutantsTestingOptions();
            MutantsCreationOptions= new MutantsCreationOptions();
            TestAssemblies = new List<TestNodeAssembly>();
            Filter = MutationFilter.AllowAll();
        }

        public IList<IMutationOperator> SelectedOperators { get; set; }
        public IList<DirectoryPathAbsolute> ProjectPaths { get; set; }
        public MutationFilter Filter { get; set; }
        public MutantsTestingOptions MutantsTestingOptions { get; set; }
        public MutantsCreationOptions MutantsCreationOptions { get; set; }
        public IList<TestNodeAssembly> TestAssemblies { get; set; }
        public OptionsModel MainOptions { get; set; }
        public CciModuleSource WhiteSource { get; set; }
    }
}