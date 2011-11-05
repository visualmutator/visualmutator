namespace VisualMutator.Model.Mutations
{
    using CommonUtilityInfrastructure;

    using ICSharpCode.Decompiler;
    using ICSharpCode.ILSpy;

    using Mono.Cecil;

    public interface IDecompiler
    {
        string DecompileType(TypeDefinition type);

        string DecompileMethod(MethodDefinition method);

        string DecompileProperty(PropertyDefinition property);

        string DecompileField(FieldDefinition field);
    }

    public class Decompiler : IDecompiler
    {
        private DecompilationOptions _opt;

        private Language _decompiler;

        public Decompiler(CodeLanguage language)
        {

            _decompiler = Functional.ValuedSwitch<CodeLanguage, Language>(language)
                .Case(CodeLanguage.CSharp, () => new CSharpLanguage())
                .Case(CodeLanguage.IL, () => new ILLanguage(true))
                .GetResult();

            _opt = new DecompilationOptions { DecompilerSettings = { ShowXmlDocumentation = false } };
        }

        public string DecompileType(TypeDefinition type)
        {
            var output = new PlainTextOutput();
            _decompiler.DecompileType(type, output, _opt);
            return output.ToString().Replace("\t", "   ");
        }

        public string DecompileMethod(MethodDefinition method)
        {
            var output = new PlainTextOutput();
            _decompiler.DecompileMethod(method, output, _opt);
            return output.ToString().Replace("\t", "   ");
        }

        public string DecompileProperty(PropertyDefinition property)
        {
            var output = new PlainTextOutput();
            _decompiler.DecompileProperty(property, output, _opt);
            return output.ToString().Replace("\t", "   ");
        }

        public string DecompileField(FieldDefinition field)
        {
            var output = new PlainTextOutput();
            _decompiler.DecompileField(field, output, _opt);
            return output.ToString().Replace("\t", "   ");
        }
    }
}