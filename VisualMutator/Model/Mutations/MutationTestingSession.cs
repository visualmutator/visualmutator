namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Controllers;
    using VisualMutator.Model.Mutations;

    public class MutationTestingSession
    {
        public MutationTestingSession()
        {
            
        }

        public IList<ExecutedOperator> MutantsGroupedByOperators { get; set; }

        public double MutantsRatio { get; set; }

        public IList<AssemblyDefinition> OriginalAssemblies { get; set; }

        public TestEnvironmentInfo TestEnvironment { get; set; }
    }
}