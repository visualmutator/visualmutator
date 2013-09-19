namespace CommonUtilityInfrastructure.DependencyInjection
{
    public interface IFactory<out TObject>
    {
        TObject Create();
        TObject CreateWithParams(params object[] parameters);
    }
}