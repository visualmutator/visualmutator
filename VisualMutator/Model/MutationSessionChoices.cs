namespace VisualMutator.Model
{
    #region

    using System;
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
            MutantsTestingOptions= new MutantsTestingOptions();
            TestAssemblies = new List<TestNodeAssembly>();
            Filter = MutationFilter.AllowAll();
        }

        public IList<IMutationOperator> SelectedOperators { get; set; }
        public MutationFilter Filter { get; set; }
        public MutantsTestingOptions MutantsTestingOptions { get; set; }
        public IList<TestNodeAssembly> TestAssemblies { get; set; }
        public DateTime SessionCreationWindowShowTime { get; set; }
    }
}