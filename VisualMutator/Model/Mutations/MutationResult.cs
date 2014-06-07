namespace VisualMutator.Model.Mutations
{
    using Microsoft.Cci;
    using StoringMutants;

    public class MutationResult
    {
        public MutationResult(IModuleSource mutatedModules, ICciModuleSource whiteModules, IMethodDefinition methodMutated)
        {
            MutatedModules = mutatedModules;
            WhiteModules = whiteModules;
            MethodMutated = methodMutated;
        }

        public ICciModuleSource WhiteModules { get; private set; }
        public IMethodDefinition MethodMutated { get; set; }
        public IModuleSource MutatedModules { get; private set; }
    }
}