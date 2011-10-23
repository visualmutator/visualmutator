namespace VisualMutator.Model.Tests
{
    using System.IO;
    using System.Reflection;

    using CommonUtilityInfrastructure.FileSystem;

    using Mono.Cecil;

    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Mutations.Types;

    using log4net;

    public interface IMutantsFileManager
    {

        void DeleteMutantFiles(StoredMutantInfo mutant);


        StoredMutantInfo StoreMutant(TestEnvironmentInfo testEnvironmentInfo, Mutant mutant);

        TestEnvironmentInfo InitTestEnvironment();
    }

 
    public class MutantsFileManager : IMutantsFileManager
    {
        private readonly IVisualStudioConnection _visualStudio;

        private readonly IAssemblyReaderWriter _assemblyReaderWriter;

        private readonly IAssembliesManager _assembliesManager;

        private readonly IFileSystem _fs;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);




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



        public StoredMutantInfo StoreMutant(TestEnvironmentInfo testEnvironmentInfo,Mutant mutant)
        {

            var result = new StoredMutantInfo();

            var assemblyDefinitions = _assembliesManager.Load(mutant.StoredAssemblies);
            foreach (AssemblyDefinition assemblyDefinition in assemblyDefinitions)
            {
                //TODO: remove: assemblyDefinition.Name.Name + ".dll", use factual original file name
                string file = Path.Combine(testEnvironmentInfo.Directory, assemblyDefinition.Name.Name + ".dll");
                _fs.File.Delete(file);
                _assemblyReaderWriter.WriteAssembly(assemblyDefinition, file);
                result.AssembliesPaths.Add(file);
            }
            return result;
        }

        public TestEnvironmentInfo InitTestEnvironment()
        {
            string mutantDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _fs.Directory.CreateDirectory(mutantDirectoryPath);

            var refer = _visualStudio.GetReferencedAssemblies();//TODO: Use mono.cecil
            foreach (var referenced in refer)
            {
                string destination = Path.Combine(mutantDirectoryPath, Path.GetFileName(referenced));
                _fs.File.Copy(referenced, destination, overwrite: true); //TODO: Remove overwrite?
            }

            return new TestEnvironmentInfo(mutantDirectoryPath);
        }

        public void DeleteMutantFiles(StoredMutantInfo mutant)
        {
           // _fs.Directory.Delete(mutant.DirectoryPath, recursive: true);
        }
       

    }

    public class TestEnvironmentInfo
    {
        public TestEnvironmentInfo(string directory)
        {
            Directory = directory;
        }

        public string Directory { get; set; }
    }
}