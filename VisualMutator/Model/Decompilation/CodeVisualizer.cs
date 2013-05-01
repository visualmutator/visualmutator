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


        string Visualize(CodeLanguage language, MutationTarget target, AssembliesProvider assemblies);
        string Visualize(CodeLanguage language, AssembliesProvider assemblies);
    }

    public class CodeVisualizer : ICodeVisualizer
    {
        private readonly ICommonCompilerAssemblies _cci;
        

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public CodeVisualizer(ICommonCompilerAssemblies cci)
        {
            _cci = cci;
        }

        public string Visualize(CodeLanguage language, AssembliesProvider assemblies)
        {
            var sb = new StringBuilder();
     
            
            foreach (var assembly in assemblies.Assemblies)
            {
                var sourceEmitterOutput = new SourceEmitterOutputString();
                var sourceEmitter = _cci.GetSourceEmitter(language, assembly, sourceEmitterOutput);
                sourceEmitter.Traverse(assembly);
                sb.Append(sourceEmitterOutput.Data);
            }  
  
            return sb.ToString();
        }

        public string Visualize(CodeLanguage language, MutationTarget target, AssembliesProvider assemblies)
        {
            
           //.// GetSourceEmitter

            var sb = new StringBuilder();
            if (target.Method != null)
            {//TODO: handle namespaces
                _log.Info("Visualize: " + target + " method: " + target.Method);
                var method = assemblies.Assemblies.SelectMany(a => a.GetAllTypes())
              .Single(t => t.Name.Value == target.Method.TypeName).Methods
              .Single(m => m.ToString() == target.Method.MethodSignature);

                var module = (IModule) TypeHelper.GetDefiningUnit(method.ContainingTypeDefinition);
                var sourceEmitterOutput = new SourceEmitterOutputString();

                var sourceEmitter = _cci.GetSourceEmitter(language, module, sourceEmitterOutput);
                sourceEmitter.Traverse(method);
       
                sb.Append(sourceEmitterOutput.Data);

               

            }
            /*  foreach (IMutationElement mutationElement in target.RetrieveNonHidden())
              {


                  var output = Switch.Into<string>().FromTypeOf(mutationElement)
                      .Case<MutationElementMethod>(elem => _decompiler.DecompileMethod(elem.FindIn(assemblies)))
                      .Case<MutationElementType>(elem => _decompiler.DecompileType(elem.FindIn(assemblies)))
                      .Case<MutationElementProperty>(elem => _decompiler.DecompileProperty(elem.FindIn(assemblies)))
                      .Case<MutationElementField>(elem => _decompiler.DecompileField(elem.FindIn(assemblies)))
                      .GetResult();

                  sb.Append(output);
              }*/
            return sb.ToString();
        }
      
    }
}