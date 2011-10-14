namespace VisualMutator.Controllers
{
    using System.Collections.Generic;

    using CommonUtilityInfrastructure.WpfUtils;

    using Mono.Cecil;

    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.TestsTree;

    public class Mutant : MutationNode
    {
        private readonly IEnumerable<AssemblyDefinition> _mutatedAssemblies;

        public Mutant(MutationNode parent, IEnumerable<AssemblyDefinition> mutatedAssemblies)
            : base(parent, "Mutant", false)
        {
            _mutatedAssemblies = mutatedAssemblies;
        }


        public IEnumerable<AssemblyDefinition> MutatedAssemblies
        {
            get
            {
                return _mutatedAssemblies;
            }
        }


        private string _stateDescription;

        public string StateDescription
        {
            get
            {
                return _stateDescription;
            }
            set
            {
                SetAndRise(ref _stateDescription, value, () => StateDescription);
            }
        }

        private TestSession _testSession;

        public TestSession TestSession
        {
            get
            {
                return _testSession;
            }
            set
            {
                SetAndRise(ref _testSession, value, () => TestSession);
            }
        }

    }
}