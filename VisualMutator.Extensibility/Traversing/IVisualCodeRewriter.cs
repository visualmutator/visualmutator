namespace VisualMutator.Extensibility
{
    public interface IVisualCodeRewriter
    {
        AstFormatter Formatter
        {
            get;
        }
    }
}