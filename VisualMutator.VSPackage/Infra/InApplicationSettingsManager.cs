namespace CommonUtilityInfrastructure
{
    #region

    using System;
    using PiotrTrzpil.VisualMutator_VSPackage.Properties;
    using UsefulTools.Core;

    #endregion

    public class InApplicationSettingsManager : ISettingsManager
    {
        private readonly Settings _settings;

        public InApplicationSettingsManager(Settings settings)
        {
            _settings = settings;
        }

        public string this[string key]
        {
            get
            {
                return (string)_settings[key];
            }
            set
            {
                _settings[key] = value;
            }
        }

        public void Initialize()
        {
            
        }

        public bool ContainsKey(string mutationresultsfilepath)
        {
            throw new NotImplementedException();
        }
    }
}