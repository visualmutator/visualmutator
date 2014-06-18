namespace VisualMutator.Model.Decompilation
{
    #region

    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using CSharpSourceEmitter;
    using log4net;
    using Microsoft.Cci;
    using Mutations;
    using Mutations.MutantsTree;
    using StoringMutants;

    #endregion

    public interface ICodeVisualizer
    {


        string Visualize(CodeLanguage language, IMethodDefinition method, ICciModuleSource moduSource);
        string Visualize(CodeLanguage language, ICciModuleSource modules);
        Task<string> VisualizeOriginalCode(CodeLanguage language, Mutant mutant);
        Task<string> VisualizeMutatedCode(CodeLanguage language, Mutant mutant);
    }

    public class CodeVisualizer : ICodeVisualizer
    {
        private readonly IMutantsCache _mutantsCache;
        private readonly MutationSessionChoices _choices;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CodeVisualizer(
            IMutantsCache mutantsCache,
            MutationSessionChoices choices)
        {
            _mutantsCache = mutantsCache;
            _choices = choices;
        }

        public async Task<string> VisualizeOriginalCode(CodeLanguage language, Mutant mutant)
        {
            var whiteCode = Visualize(language, mutant.MutationTarget.MethodRaw, _choices.WhiteSource.First());
            return whiteCode;
        }

        public async Task<string> VisualizeMutatedCode(CodeLanguage language, Mutant mutant)
        {
            MutationResult mutationResult = await _mutantsCache.GetMutatedModulesAsync(mutant);

            var result = Visualize(language, mutationResult.MethodMutated, mutationResult.MutatedModules);
          //  _mutantsCache.Release(mutationResult);
            return result;
        }

        public string Visualize(CodeLanguage language, IMethodDefinition method, ICciModuleSource moduSource)
        {
            if (method == null)
            {
                return "No method to visualize.";
            }
            _log.Info("Visualize: " + method);
            var module = (IModule)TypeHelper.GetDefiningUnit(method.ContainingTypeDefinition);
            var sourceEmitterOutput = new SourceEmitterOutputString();
            var sourceEmitter = moduSource.GetSourceEmitter(language, module, sourceEmitterOutput);
            sourceEmitter.Traverse(method);
            return sourceEmitterOutput.Data;
        }


        public string Visualize(CodeLanguage language, ICciModuleSource modules)
        {
            var sb = new StringBuilder();

            foreach (var assembly in modules.Modules)
            {
                var sourceEmitterOutput = new SourceEmitterOutputString();
                var sourceEmitter = modules.GetSourceEmitter(language, assembly.Module, sourceEmitterOutput);
                sourceEmitter.Traverse(assembly.Module);
                sb.Append(sourceEmitterOutput.Data);
            }

            return sb.ToString();
        }


      


    }
}