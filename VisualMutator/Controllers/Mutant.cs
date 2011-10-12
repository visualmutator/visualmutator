namespace VisualMutator.Controllers
{
    using System.Collections.Generic;

    using CommonUtilityInfrastructure.WpfUtils;

    using Mono.Cecil;

    using VisualMutator.Model.Mutations;

    public class Mutant : ModelElement
    {
        private readonly IEnumerable<AssemblyDefinition> _mutatedAssemblies;

        public IEnumerable<AssemblyDefinition> MutatedAssemblies
        {
            get
            {
                return _mutatedAssemblies;
            }
        }

        private MutantResultState _resultState;

        public MutantResultState ResultState
        {
            get
            {
                return _resultState;
            }
            set
            {
                SetAndRise(ref _resultState, value, () => ResultState);
            }
        }

        public Mutant(IEnumerable<AssemblyDefinition> mutatedAssemblies)
        {
            _mutatedAssemblies = mutatedAssemblies;
        }
    }
}