namespace VisualMutator.Model.Decompilation
{
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using CSharpSourceEmitter;
    using Microsoft.Cci;
    using VisualMutator.Extensibility;
    using log4net;

    public interface ICodeVisualizer
    {


        string Visualize(CodeLanguage language, IMethodDefinition method, ModulesProvider modules);
        string Visualize(CodeLanguage language, ModulesProvider modules);
    }

    public class CodeVisualizer : ICodeVisualizer
    {
        private readonly ICommonCompilerAssemblies _cci;
        

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public CodeVisualizer(ICommonCompilerAssemblies cci)
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
                return "";
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