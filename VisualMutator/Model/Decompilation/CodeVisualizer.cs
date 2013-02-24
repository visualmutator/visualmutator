namespace VisualMutator.Model.Decompilation
{
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using VisualMutator.Extensibility;
    using log4net;

    public interface ICodeVisualizer
    {


        CodePair CreateCodesToCompare(MutationTarget target, AssembliesProvider originalAssemblies,
            AssembliesProvider mutatedAssemblies);
    }

    public class CodeVisualizer : ICodeVisualizer
    {
        private IDecompiler _decompiler;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public CodeVisualizer(CodeLanguage language)
        {
            _decompiler = new Decompiler(language);
        }



        public string Visualize(MutationTarget target, AssembliesProvider assemblies)
        {

            var sb = new StringBuilder();
            if (target.Method != null)
            {
                _log.Info("Visualize: " + target + " method: " + target.Method);
                var method = assemblies.Assemblies.SelectMany(a => a.MainModule.Types)
              .Single(t => t.Name == target.Method.TypeName).Methods
              .Single(m => m.Name == target.Method.MethodName);

                sb.Append(_decompiler.DecompileMethod(method));

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
        public CodePair CreateCodesToCompare(MutationTarget target,
            AssembliesProvider originalAssemblies, AssembliesProvider mutatedAssemblies)
        {
            return new CodePair
            {
                OriginalCode = Visualize(target, originalAssemblies),
                MutatedCode = Visualize(target, mutatedAssemblies),
            };


        }
    }
}