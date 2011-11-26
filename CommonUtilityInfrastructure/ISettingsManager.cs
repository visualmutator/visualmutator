namespace CommonUtilityInfrastructure
{
    public interface ISettingsManager
    {

        string this[string key]
        {
            get; set;
        }

        void Initialize();
    }
}