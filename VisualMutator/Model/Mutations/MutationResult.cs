namespace VisualMutator.Model.Mutations
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Cci;
    using MutantsTree;
    using StoringMutants;

    public class MutationResult
    {
        private readonly Mutant _mutant;

        public Mutant Mutant
        {
            get { return _mutant; }
        }

        public MutationResult(Mutant mutant, ICciModuleSource mutatedModules, IMethodDefinition methodMutated)
        {
            _mutant = mutant;
            MutatedModules = mutatedModules;
            MethodMutated = methodMutated;
        }

        public ICciModuleSource MutatedModules { get; private set; }
        public IMethodDefinition MethodMutated { get; set; }

     

    }
}