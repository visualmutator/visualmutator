namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    public interface IFactory<out TObject>
    {
        TObject Create();
    }
}