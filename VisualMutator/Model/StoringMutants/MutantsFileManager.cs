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


        void WriteMutantsToDisk(string rootFolder, IList<AssemblyNode> mutantsInOperators, System.Action<Mutant, StoredMutantInfo> verify, ProgressCounter onSavingProgress);
        StoredMutantInfo StoreMutant(string directory, ModulesProvider mutant);

        StoredMutantInfo StoreMutant(string directory, Mutant mutant);
    }

 
    public class MutantsFileManager : IMutantsFileManager
    {
       
    

        private readonly IMutantsCache _mutantsCache;

        private readonly IModuleSource _moduleSource;

        private readonly IFileSystem _fs;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);




        public MutantsFileManager(
            IMutantsCache mutantsCache,
            IModuleSource moduleSource,
            IFileSystem fs)
        {
         
            _mutantsCache = mutantsCache;
            _moduleSource = moduleSource;
            _fs = fs;
        }


        public void WriteMutantsToDisk(string rootFolder, IList<AssemblyNode> mutantsInOperators, 
            System.Action<Mutant, StoredMutantInfo> verify, ProgressCounter onSavingProgress)
        {

           
          //  var storedMutantsList = new List<Tuple<Mutant, StoredMutantInfo>>();

            onSavingProgress.Initialize(mutantsInOperators.Cast<CheckedNode>().SelectManyRecursive(
                    g => g.Children??new NotifyingCollection<CheckedNode>(), leafsOnly:true).Count(),
                    ProgressMode.Before);

            foreach (AssemblyNode assembly in mutantsInOperators)
            {
                string subFolder = Path.Combine(rootFolder, assembly.Name.RemoveInvalidPathCharacters());
                
                _fs.Directory.CreateDirectory(subFolder);
                //TODO: to subfolders by group
                foreach (Mutant mutant in assembly.Children.SelectManyRecursive(
                    g => g.Children??new NotifyingCollection<CheckedNode>(), leafsOnly:true) )
                {
                    onSavingProgress.Progress();
                    try
                    {
                        mutant.State = MutantResultState.Creating;
                        ModulesProvider assembliesProvider = _mutantsCache.GetMutatedModules(mutant);
                    
                        string mutantFolder = Path.Combine(subFolder,mutant.Id.ToString());
                        _fs.Directory.CreateDirectory(mutantFolder);



                        StoredMutantInfo storedMutantInfo = StoreMutant(mutantFolder, assembliesProvider);
                        verify(mutant, storedMutantInfo);
                    }
                    catch (Exception e)
                    {
                        mutant.MutantTestSession.ErrorDescription = "Error ocurred";
                        mutant.MutantTestSession.ErrorMessage = e.Message;
                        mutant.MutantTestSession.Exception = e;
                        mutant.State = MutantResultState.Error;
                        _log.Error("Set mutant " + mutant.Id + " error: " + mutant.State + " message: " + e.Message);
                    }


                    mutant.State = MutantResultState.Live;
                   // storedMutantsList.Add(Tuple.Create(mutant,));
                }
                
            }
        }


        public StoredMutantInfo StoreMutant(string directory, ModulesProvider assembliesProvider)
        {

            
            var result = new StoredMutantInfo();
            
            foreach (IModule module in assembliesProvider.Assemblies)
            {
                
                //TODO: remove: assemblyDefinition.Name.Name + ".dll", use factual original file name
                string file = Path.Combine(directory, module.Name.Value + ".dll");
                _moduleSource.WriteToFile(module, file);
                result.AssembliesPaths.Add(file);
            }


            return result;

        }


        public StoredMutantInfo StoreMutant(string directory, Mutant mutant)
        {
            return StoreMutant(directory, _mutantsCache.GetMutatedModules(mutant));

        }

    }

}