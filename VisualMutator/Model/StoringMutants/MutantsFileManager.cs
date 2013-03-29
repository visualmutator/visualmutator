namespace VisualMutator.Model.StoringMutants
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using CommonUtilityInfrastructure.FileSystem;
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Paths;
    using Microsoft.Cci;
    using Mutations.MutantsTree;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests.Custom;
    using log4net;

    public interface IMutantsFileManager
    {


        StoredMutantInfo StoreMutant(string directory, Mutant mutant);

        TestEnvironmentInfo InitTestEnvironment(MutationTestingSession currentSession);

        void CleanupTestEnvironment(TestEnvironmentInfo info);

        void WriteMutantsToDisk(string rootFolder, IList<ExecutedOperator> mutantsInOperators, System.Action<Mutant, StoredMutantInfo> verify, ProgressCounter onSavingProgress);

        void OnTestingCancelled();
    }

 
    public class MutantsFileManager : IMutantsFileManager
    {
        private readonly IHostEnviromentConnection _hostEnviroment;

    

        private readonly IMutantsCache _mutantsCache;

        private readonly ICommonCompilerAssemblies _commonCompilerAssemblies;

        private readonly IFileSystem _fs;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private AspNetTestingProcessExtension prep = new AspNetTestingProcessExtension();


        public MutantsFileManager(
            IHostEnviromentConnection hostEnviroment,
            IMutantsCache mutantsCache,
            ICommonCompilerAssemblies commonCompilerAssemblies,
            IFileSystem fs)
        {
            _hostEnviroment = hostEnviroment;

            _mutantsCache = mutantsCache;
            _commonCompilerAssemblies = commonCompilerAssemblies;
            _fs = fs;
        }


        public void WriteMutantsToDisk(string rootFolder, 
            IList<ExecutedOperator> mutantsInOperators, System.Action<Mutant, StoredMutantInfo> verify, 
            ProgressCounter onSavingProgress)
        {

           
          //  var storedMutantsList = new List<Tuple<Mutant, StoredMutantInfo>>();

            onSavingProgress.Initialize(mutantsInOperators.Select(oper => oper.Children)
                .Sum(ch => ch.Count), ProgressMode.Before);
         
            foreach (ExecutedOperator oper in mutantsInOperators)
            {
                string subFolder = Path.Combine(rootFolder, oper.Identificator.RemoveInvalidPathCharacters());
                
                _fs.Directory.CreateDirectory(subFolder);
                foreach (Mutant mutant in oper.Mutants)
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

        public void OnTestingCancelled()
        {
         //   prep.OnTestingCancelled();
        }
        public IEnumerable<string> GetReferencedAssemblies()
        {
            var projects = _hostEnviroment.GetProjectAssemblyPaths().ToList();
    
            var list = new List<string>();
            foreach (var binDir in projects.Select(p => p.ParentDirectoryPath))
            {
                var files = Directory.GetFiles(binDir.Path, "*.dll", SearchOption.AllDirectories)
                    .Where(p => !projects.Contains(p.ToFilePathAbsolute()));
                list.AddRange(files);
            }
            return list;
        }
        public TestEnvironmentInfo InitTestEnvironment(MutationTestingSession currentSession)
        {
       
            string mutantDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _fs.Directory.CreateDirectory(mutantDirectoryPath);

            var refer = GetReferencedAssemblies();//TODO: Use better way
            foreach (var referenced in refer)
            {
                string destination = Path.Combine(mutantDirectoryPath, Path.GetFileName(referenced));
                _fs.File.Copy(referenced, destination, overwrite: true); //TODO: Remove overwrite?
            }

            foreach (string path in currentSession.Choices.MutantsCreationOptions.AdditionalFilesToCopy)
            {
                string destination = Path.Combine(mutantDirectoryPath, Path.GetFileName(path));
                _fs.File.Copy(path, destination, overwrite: true); 
            }
            return new TestEnvironmentInfo(mutantDirectoryPath);
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
                _commonCompilerAssemblies.WriteToFile(module, file);
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

       

        public void CleanupTestEnvironment(TestEnvironmentInfo info)
        {

            if (info != null && _fs.Directory.Exists(info.DirectoryPath))
            {
                try
                {
                    _fs.Directory.Delete(info.DirectoryPath, recursive: true);
                }
                catch (UnauthorizedAccessException e)
                {
                    _log.Warn(e);
                }
                
            }
        }

    

    }

    public class TestEnvironmentInfo
    {
        public TestEnvironmentInfo(string directory)
        {
            DirectoryPath = directory;
        }

        public string DirectoryPath { get; set; }
    }
}