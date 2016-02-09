namespace VisualMutator.Model.Decompilation
{
    #region

    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using CodeDifference;
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
        Task<CodeWithDifference> CreateDifferenceListing(CodeLanguage language, Mutant mutant, MutationResult mutationResult);
    }

    public class CodeVisualizer : ICodeVisualizer
    {
        private readonly OriginalCodebase _originalCodebase;
        private readonly ICodeDifferenceCreator _differenceCreator;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CodeVisualizer(
            OriginalCodebase originalCodebase,
            ICodeDifferenceCreator differenceCreator)
        {
            _originalCodebase = originalCodebase;
            _differenceCreator = differenceCreator;
        }

        public async Task<CodeWithDifference> CreateDifferenceListing(CodeLanguage language, Mutant mutant, MutationResult mutationResult)
        {
            _log.Debug("CreateDifferenceListing in object: " + ToString() + GetHashCode());
            try
            {
                
                var whiteCode = await VisualizeOriginalCode(language, mutant);
                var mutatedCode = await VisualizeMutatedCode(language, mutationResult);
                CodePair pair = new CodePair
                {
                    OriginalCode = whiteCode,
                    MutatedCode = mutatedCode
                };
                return _differenceCreator.GetDiff(language, pair.OriginalCode, pair.MutatedCode);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return new CodeWithDifference
                {
                    Code = "Exception occurred while decompiling: " + e,
                    LineChanges = Enumerable.Empty<LineChange>().ToList()
                };
            }
        }


        public async Task<string> VisualizeOriginalCode(CodeLanguage language, Mutant mutant)
        {
            var whiteCode = Visualize(language, mutant.MutationTarget.MethodRaw,
                _originalCodebase.Modules.Single(m => m.Module.Name == mutant.MutationTarget.ProcessingContext.ModuleName));

            if (mutant._mutationTargets.Count != 0 && mutant.MutationTarget.MethodRaw != mutant._mutationTargets[0].MethodRaw)
            {
                whiteCode += Visualize(language, mutant._mutationTargets[0].MethodRaw,
                _originalCodebase.Modules.Single(m => m.Module.Name == mutant._mutationTargets[0].ProcessingContext.ModuleName));
            }
            return whiteCode;
        }

        public async Task<string> VisualizeMutatedCode(CodeLanguage language, MutationResult mutationResult)
        {
            var result = Visualize(language, mutationResult.MethodMutated, mutationResult.MutatedModules);//oryginalnie była tylko ta linijka i return

            if (mutationResult.AdditionalMethodsMutated != null && mutationResult.MethodMutated!=mutationResult.AdditionalMethodsMutated[0])
            {
                result += Visualize(language, mutationResult.AdditionalMethodsMutated[0], mutationResult.MutatedModules);
            }
            //  _mutantsCache.Release(mutationResult);*/
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
        
        public string Visualize(CodeLanguage language, IMethodDefinition mainMethod, System.Collections.Generic.List<IMethodDefinition> addMethods, ICciModuleSource moduSource)
        {
            if (mainMethod == null)
            {
                return "No method to visualize.";
            }
            _log.Info("Visualize: " + mainMethod);
            var module = (IModule)TypeHelper.GetDefiningUnit(addMethods[0].ContainingTypeDefinition);
            var sourceEmitterOutput = new SourceEmitterOutputString();
            var sourceEmitter = moduSource.GetSourceEmitter(language, module, sourceEmitterOutput);
            sourceEmitter.Traverse(addMethods[0]);

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