namespace VisualMutator.Extensibility
{
    using Microsoft.Cci;

    public interface IOperatorUtils
    {
        IModule CompileModuleFromCode(string code, IMetadataReaderHost host);
        AstFormatter Formatter { get; }
    }
}