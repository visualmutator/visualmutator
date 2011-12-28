namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using PiotrTrzpil.VisualMutator_VSPackage.Properties;

    public class InApplicationSettingsManager : ISettingsManager
    {
        private readonly Settings _settings;

        public InApplicationSettingsManager(PiotrTrzpil.VisualMutator_VSPackage.Properties.Settings settings)
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