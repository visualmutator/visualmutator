namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.VisualStudio.Settings;

    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.Properties;

    using VisualMutator.Infrastructure;

    public class VisualStudioSettingsProvider : ISettingsManager
    {
        private readonly VisualStudioConnection _visualStudioConnection;

  //      private readonly Settings _settings;

        private WritableSettingsStore userSettingsStore;


        private const string CollectionName = "VisualMutator Settings";

        public VisualStudioSettingsProvider(VisualStudioConnection visualStudioConnection)
        {
            _visualStudioConnection = visualStudioConnection;
           


            
   
        }


        public void Initialize()
        {
            userSettingsStore = _visualStudioConnection.SettingsManager
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

        public string this[string key]
        {
            get
            {
                Throw.IfArgumentNull(key);
                return userSettingsStore.GetString(CollectionName, key);
            }
            set
            {
                Throw.IfArgumentNull(key);
                Throw.IfArgumentNull(value);
                userSettingsStore.SetString(CollectionName, key,value);
            }
        }
    }
}