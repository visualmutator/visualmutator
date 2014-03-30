namespace VisualMutator.Model.StoringMutants
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Infrastructure;
    using log4net;
    using Microsoft.Cci;
    using Mutations.MutantsTree;
    using Mutations.Types;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;

    #endregion

    public interface IMutantsFileManager
    {
        void StoreMutant(StoredMutantInfo directory, IModuleSource mutant);

        void StoreMutant(StoredMutantInfo directory, Mutant mutant);
    }

 
    public class MutantsFileManager : IMutantsFileManager
    {
        private readonly IMutantsCache _mutantsCache;

        private readonly ICciModuleSource _moduleSource;

        private readonly IFileSystem _fs;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MutantsFileManager(
            IMutantsCache mutantsCache,
            ICciModuleSource moduleSource,
            IFileSystem fs)
        {
         
            _mutantsCache = mutantsCache;
            _moduleSource = moduleSource;
            _fs = fs;
        }

        public void StoreMutant(StoredMutantInfo info, IModuleSource assembliesProvider)
        {
            
            
            foreach (IModule module in assembliesProvider.Modules)
            {
                
                //TODO: remove: assemblyDefinition.Name.Name + ".dll", use factual original file name
                string file = Path.Combine(info.Directory, module.Name.Value + ".dll");
                _moduleSource.WriteToFile(module, file);
                info.AssembliesPaths.Add(file);
            }
        }


        public void StoreMutant(StoredMutantInfo info, Mutant mutant)
        {
            StoreMutant(info, _mutantsCache.GetMutatedModules(mutant));

        }

    }

}