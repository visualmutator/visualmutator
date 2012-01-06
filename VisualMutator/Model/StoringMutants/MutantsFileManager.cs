namespace VisualMutator.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CommonUtilityInfrastructure.FileSystem;
    using CommonUtilityInfrastructure;

    using Mono.Cecil;

    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests.Custom;

    using log4net;

    public interface IMutantsFileManager
    {


        StoredMutantInfo StoreMutant(string directory, Mutant mutant);

        TestEnvironmentInfo InitTestEnvironment(MutationTestingSession currentSession);

        void CleanupTestEnvironment(TestEnvironmentInfo info);

        void WriteMutantsToDisk(string rootFolder, IList<ExecutedOperator> mutantsInOperators, Action<Mutant, StoredMutantInfo> verify, ProgressCounter onSavingProgress);

        void OnTestingCancelled();
    }

 
    public class MutantsFileManager : IMutantsFileManager
    {
        private readonly IVisualStudioConnection _visualStudio;

        private readonly IAssemblyReaderWriter _assemblyReaderWriter;

        private readonly IAssembliesManager _assembliesManager;

        private readonly IFileSystem _fs;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private DefaultTestEnvPreparer prep = new DefaultTestEnvPreparer();


        public MutantsFileManager(
            IVisualStudioConnection visualStudio,
            IAssemblyReaderWriter assemblyReaderWriter,
            IAssembliesManager assembliesManager,
            IFileSystem fs)
        {
            _visualStudio = visualStudio;
            _assemblyReaderWriter = assemblyReaderWriter;
            _assembliesManager = assembliesManager;
            _fs = fs;
        }


        public void WriteMutantsToDisk(string rootFolder, 
            IList<ExecutedOperator> mutantsInOperators, Action<Mutant, StoredMutantInfo> verify, 
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

        public TestEnvironmentInfo InitTestEnvironment(MutationTestingSession currentSession)
        {
         //   prep.Initialize(@"D:\PLIKI\Programowanie\C#\Source Code'y\ASP.NET MVC\MvcMusicStore 3\MvcMusicStore|2789");

            string mutantDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _fs.Directory.CreateDirectory(mutantDirectoryPath);

            var refer = _visualStudio.GetReferencedAssemblies();//TODO: Use mono.cecil
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

            IList<AssemblyDefinition> assemblyDefinitions = _assembliesManager.Load(mutant.StoredAssemblies);
            foreach (AssemblyDefinition assemblyDefinition in assemblyDefinitions)
            {
                //TODO: remove: assemblyDefinition.Name.Name + ".dll", use factual original file name
                string file = Path.Combine(directory, assemblyDefinition.Name.Name + ".dll");
                _fs.File.Delete(file);
                _assemblyReaderWriter.WriteAssembly(assemblyDefinition, file);
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