namespace CommonUtilityInfrastructure.DependencyInjection
{
    public interface IFactory<out TObject>
    {
        TObject Create(params object[] parameters);
    }
}