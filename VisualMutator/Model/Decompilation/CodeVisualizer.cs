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
        Task<StoredMutantInfo> StoreMutant(Mutant mutant);
    }

    public class CodeVisualizer : ICodeVisualizer
    {
        private readonly IProjectClonesManager _clonesManager;
        private readonly IMutantsCache _mutantsCache;
        private readonly MutationSessionChoices _choices;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CodeVisualizer(
            IProjectClonesManager clonesManager,
            IMutantsCache mutantsCache,
            MutationSessionChoices choices)
        {
            _clonesManager = clonesManager;
            _mutantsCache = mutantsCache;
            _choices = choices;
        }

        public async Task<string> VisualizeOriginalCode(CodeLanguage language, Mutant mutant)
        {
            var whiteCode = Visualize(language, mutant.MutationTarget.MethodRaw, _choices.WhiteSource);
            return whiteCode;
        }
        public async Task<string> VisualizeMutatedCode(CodeLanguage language, Mutant mutant)
        {
            MutationResult mutationResult = await _mutantsCache.GetMutatedModulesAsync(mutant);

            var result = Visualize(language, mutationResult.MethodMutated, mutationResult.WhiteModules);
            _mutantsCache.Release(mutationResult);
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


        public async Task<StoredMutantInfo> StoreMutant(Mutant mutant)
        {
            mutant.State = MutantResultState.Creating;

            var mutationResult = await _mutantsCache.GetMutatedModulesAsync(mutant);

            mutant.State = MutantResultState.Writing;

            var clone = await _clonesManager.CreateCloneAsync("InitTestEnvironment");
            var info = new StoredMutantInfo(clone);

            var singleMutated = mutationResult.MutatedModules.Modules.SingleOrDefault();
            if (singleMutated != null)
            {
                //TODO: remove: assemblyDefinition.Name.Name + ".dll", use factual original file name
                string file = Path.Combine(info.Directory, singleMutated.Name + ".dll");

                var memory = mutationResult.WhiteModules.WriteToStream(singleMutated, file);
                _mutantsCache.Release(mutationResult);

                using (FileStream peStream = File.Create(file))
                {
                    await memory.CopyToAsync(peStream);
                }
                
                info.AssembliesPaths.Add(file);
            }

            var otherModules = _choices.WhiteSource.ModulesInfo
                .Where(_ => singleMutated == null || _.Name != singleMutated.Name);

            foreach (var otherModule in otherModules)
            {
                string file = Path.Combine(info.Directory, otherModule.Name + ".dll");
                info.AssembliesPaths.Add(file);
            }

            return info;
        }


    }
}