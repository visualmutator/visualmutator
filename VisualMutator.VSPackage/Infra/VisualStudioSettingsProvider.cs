namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FunctionalUtils;
    using Microsoft.VisualStudio.Settings;

    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.Properties;

    using VisualMutator.Infrastructure;

    public class VisualStudioSettingsProvider : ISettingsManager
    {
        private readonly VisualStudioConnection _hostEnviromentConnection;

  //      private readonly Settings _settings;

        private WritableSettingsStore userSettingsStore;


        private const string CollectionName = "VisualMutator Settings";

        public VisualStudioSettingsProvider(VisualStudioConnection hostEnviromentConnection)
        {
            _hostEnviromentConnection = hostEnviromentConnection;
           


            
   
        }


        public void Initialize()
        {
            userSettingsStore = _hostEnviromentConnection.SettingsManager
                .GetWritableSettingsStore(SettingsScope.UserSettings);

            if (!userSettingsStore.CollectionExists(CollectionName))
            {
                userSettingsStore.CreateCollection(CollectionName);

            }


          //  if (!userSettingsStore.PropertyExists(CollectionName, "MutationResultsFilePath"))
         //   {
          //      this["MutationResultsFilePath"] = @"C:\results";
           // }
            


        }


        public bool ContainsKey(string key)
        {
            return userSettingsStore.PropertyExists(CollectionName, key);
        }




        public string this[string key]
        {
            get
            {
                Throw.IfNull(key);
                return userSettingsStore.GetString(CollectionName, key);
            }
            set
            {
                Throw.IfNull(key);
                Throw.IfNull(value);
                userSettingsStore.SetString(CollectionName, key,value);
            }
        }
    }
}