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


        CodePair CreateCodesToCompare(CodeLanguage language,  MutationTarget target, AssembliesProvider originalAssemblies,
            AssembliesProvider mutatedAssemblies);
    }

    public class CodeVisualizer : ICodeVisualizer
    {
        private readonly CommonCompilerAssemblies _cci;
        

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public CodeVisualizer(CommonCompilerAssemblies cci)
        {
            _cci = cci;
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
              .Single(m => m.Name.Value == target.Method.MethodName);

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
        public CodePair CreateCodesToCompare(CodeLanguage language, MutationTarget target,
            AssembliesProvider originalAssemblies, AssembliesProvider mutatedAssemblies)
        {
            return new CodePair
            {
                OriginalCode = Visualize(language, target, originalAssemblies),
                MutatedCode = Visualize(language, target, mutatedAssemblies),
            };


        }
    }
}