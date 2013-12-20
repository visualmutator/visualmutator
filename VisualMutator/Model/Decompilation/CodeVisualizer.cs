namespace VisualMutator.Model.Decompilation
{
    #region

    using System.Reflection;
    using System.Text;
    using CSharpSourceEmitter;
    using log4net;
    using Microsoft.Cci;

    #endregion

    public interface ICodeVisualizer
    {


        string Visualize(CodeLanguage language, IMethodDefinition method, ModulesProvider modules);
        string Visualize(CodeLanguage language, ModulesProvider modules);
    }

    public class CodeVisualizer : ICodeVisualizer
    {
        private readonly IModuleSource _cci;
        

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public CodeVisualizer(IModuleSource cci)
        {
            _cci = cci;
        }

        public string Visualize(CodeLanguage language, ModulesProvider modules)
        {
            var sb = new StringBuilder();
     
            
            foreach (var assembly in modules.Assemblies)
            {
                var sourceEmitterOutput = new SourceEmitterOutputString();
                var sourceEmitter = _cci.GetSourceEmitter(language, assembly, sourceEmitterOutput);
                sourceEmitter.Traverse(assembly);
                sb.Append(sourceEmitterOutput.Data);
            }  
  
            return sb.ToString();
        }
        
        public string Visualize(CodeLanguage language, IMethodDefinition method, ModulesProvider modules)
        {
            if (method == null)
            {
                return "No method to visualize.";
            }

            var sb = new StringBuilder();
            _log.Info("Visualize: " + method);
            var module = (IModule) TypeHelper.GetDefiningUnit(method.ContainingTypeDefinition);
            var sourceEmitterOutput = new SourceEmitterOutputString();

            var sourceEmitter = _cci.GetSourceEmitter(language, module, sourceEmitterOutput);
            sourceEmitter.Traverse(method);
       
            sb.Append(sourceEmitterOutput.Data);

               

            return sb.ToString();
        }
      
    }
}