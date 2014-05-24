namespace VisualMutator.Model.Decompilation
{
    #region

    using System.Reflection;
    using System.Text;
    using CSharpSourceEmitter;
    using log4net;
    using Microsoft.Cci;
    using Mutations;
    using StoringMutants;

    #endregion

    public interface ICodeVisualizer
    {


        string Visualize(CodeLanguage language, IMethodDefinition method, MutationResult cci);
        string Visualize(CodeLanguage language, ICciModuleSource modules);
    }

    public class CodeVisualizer : ICodeVisualizer
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


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
        
        public string Visualize(CodeLanguage language, IMethodDefinition method, MutationResult result)
        {
            if (method == null)
            {
                return "No method to visualize.";
            }
            _log.Info("Visualize: " + method);
            var module = (IModule) TypeHelper.GetDefiningUnit(method.ContainingTypeDefinition);
            var sourceEmitterOutput = new SourceEmitterOutputString();

            var sourceEmitter = result.WhiteModules.GetSourceEmitter(language, module, sourceEmitterOutput);
            sourceEmitter.Traverse(method);
       
            return sourceEmitterOutput.Data;
        }
      
    }
}