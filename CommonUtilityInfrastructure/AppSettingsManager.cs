namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using FunctionalUtils;


    public class AppSettingsManager : ISettingsManager
    {
        private NameValueCollection _settings;


        public AppSettingsManager()
        {
          
        }

        public string this[string key]
        {
            get
            {
                var val = _settings[key];
                if(val == null)
                {
                    throw new KeyNotFoundException("Key: "+ key +" not found in settings.");
                }
                return val;
            }
            set
            {
                Throw.IfNull(value);
                _settings[key] = value;
            }
        }

        public void Initialize()
        {

            _settings = ConfigurationManager.AppSettings;
        }

        public bool ContainsKey(string key)
        {
            return _settings[key] != null;
        }
    }
}