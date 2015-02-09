namespace VisualMutator.Tests.Operators
{
    using System.Collections.Generic;
    using UsefulTools.Core;

    public class MapSettingsManager : ISettingsManager
    {
        private Dictionary<string, string> _settings;
        public MapSettingsManager()
        {
            _settings = new Dictionary<string, string>();
        }

        public void Initialize()
        {

        }

        public bool ContainsKey(string key)
        {
            return _settings.ContainsKey(key);
        }

        public string this[string key]
        {
            get
            {
                return _settings[key];
            }
            set
            {
                _settings[key] = value;
            }
        }
    }
}