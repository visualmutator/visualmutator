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
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;

    #endregion

    public interface IMutantsFileManager
    {


        void WriteMutantsToDisk(string rootFolder, IList<AssemblyNode> mutantsInOperators, System.Action<Mutant, StoredMutantInfo> verify, ProgressCounter onSavingProgress);
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


        public void WriteMutantsToDisk(string rootFolder, IList<AssemblyNode> mutantsInOperators, System.Action<Mutant, StoredMutantInfo> verify, ProgressCounter onSavingProgress)
        {

           
          //  var storedMutantsList = new List<Tuple<Mutant, StoredMutantInfo>>();

            onSavingProgress.Initialize(mutantsInOperators.Select(oper => oper.Children)
                .Sum(ch => ch.Count), ProgressMode.Before);
         
            foreach (ExecutedOperator oper in mutantsInOperators)
            {
                string subFolder = Path.Combine(rootFolder, oper.Identificator.RemoveInvalidPathCharacters());
                
                _fs.Directory.CreateDirectory(subFolder);
                //TODO: to subfolders by group
                foreach (Mutant mutant in oper.MutantGroups.SelectMany(g=>g.Mutants))
                {
                    onSavingProgress.Progress();

                    string mutantFolder = Path.Combine(subFolder,mutant.Id.ToString());
                    _fs.Directory.CreateDirectory(mutantFolder);
                    StoredMutantInfo storedMutantInfo = StoreMutant(mutantFolder, mutant);
                    verify(mutant, storedMutantInfo);
                   // storedMutantsList.Add(Tuple.Create(mutant,));
                }
                
            }
         //   return storedMutantsList;
        }

  
        public StoredMutantInfo StoreMutant(string directory, Mutant mutant)
        {

            
            var result = new StoredMutantInfo();
            var assembliesProvider = _mutantsCache.GetMutatedModules(mutant);
            //IList<AssemblyDefinition> assemblyDefinitions = _assembliesManager.Load(mutant.StoredAssemblies).Assemblies;
            foreach (IModule module in assembliesProvider.Assemblies)
            {
                
                //TODO: remove: assemblyDefinition.Name.Name + ".dll", use factual original file name
                string file = Path.Combine(directory, module.Name.Value + ".dll");
                _moduleSource.WriteToFile(module, file);
               // _fs.File.Delete(file);
            //    _assemblyReaderWriter.WriteAssembly(assemblyDefinition, file);
                result.AssembliesPaths.Add(file);
            }
     

/*
            prep.PrepareForMutant(directory, (dest) =>
            {
                foreach (var p in result.AssembliesPaths)
                {
                    _fs.File.Copy(p, Path.Combine(dest, Path.GetFileName(p)),true);
                }

            });
*/



            return result;

        }

       
    

    }

}