namespace VisualMutator.Infrastructure.Factories
{
    public interface IFactory<out TObject>
    {
        TObject Create();
    }
}