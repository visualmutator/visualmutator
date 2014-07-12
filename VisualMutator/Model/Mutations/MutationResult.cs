namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Documents;
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

        public MutationResult(Mutant mutant, ICciModuleSource mutatedModules, List<CciModuleSource> old, IMethodDefinition methodMutated)
        {
            _mutant = mutant;
            MutatedModules = mutatedModules;
            Old = old;
            MethodMutated = methodMutated;
        }

        public ICciModuleSource MutatedModules { get; private set; }
        public List<CciModuleSource> Old { get; set; }
        public IMethodDefinition MethodMutated { get; set; }

     

    }
}