namespace VisualMutator.Controllers
{
    using System.Collections.Generic;

    using Mono.Cecil;

    public class Mutant
    {
        private readonly IEnumerable<AssemblyDefinition> _mutatedAssemblies;

        public IEnumerable<AssemblyDefinition> MutatedAssemblies
        {
            get
            {
                return _mutatedAssemblies;
            }
        }

        public Mutant(IEnumerable<AssemblyDefinition> mutatedAssemblies)
        {
            _mutatedAssemblies = mutatedAssemblies;
        }
    }
}