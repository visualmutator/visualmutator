namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Serialization;

    using CommonUtilityInfrastructure.FileSystem;

    using Mono.Cecil;

    using VisualMutator.Controllers;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations.Types;

    using log4net;

    public interface IMutantsFileManager
    {

        void DeleteMutantFiles(StoredMutantInfo mutant);

      
        StoredMutantInfo StoreMutant(Mutant mutant);
    }

    public class MutantsFileManager : IMutantsFileManager
    {
        private readonly IVisualStudioConnection _visualStudio;

        private readonly IAssemblyReaderWriter _assemblyReaderWriter;

        private readonly IFileSystem _fs;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);




        public MutantsFileManager(
            IVisualStudioConnection visualStudio,
            IAssemblyReaderWriter assemblyReaderWriter,
            IFileSystem fs)
        {
            _visualStudio = visualStudio;
            _assemblyReaderWriter = assemblyReaderWriter;
            _fs = fs;
        }

        public string MutantDirectoryPath()
        {
            return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
   
        }
/*
        public void StoreMutant(StoredMutantInfo mutant, IEnumerable<AssemblyDefinition> assemblies)
        {
            string mutantDirectoryPath = MutantDirectoryPath(mutant);
            _fs.Directory.CreateDirectory(mutantDirectoryPath);

            var refer = _visualStudio.GetReferencedAssemblies();
            foreach (var referenced in refer)
            {
                string destination = Path.Combine(mutantDirectoryPath, Path.GetFileName(referenced));
                _fs.File.Copy(referenced, destination, overwrite: true);
            }

            foreach (AssemblyDefinition assemblyDefinition in assemblies)
            {
                string file = Path.Combine(mutantDirectoryPath, assemblyDefinition.Name.Name + ".dll");
                _fs.File.Delete(file);
                _assemblyReaderWriter.WriteAssembly(assemblyDefinition, file);
                mutant.AssembliesPaths.Add(file);
            }
        }*/
        public StoredMutantInfo StoreMutant(Mutant mutant)
        {
            string mutantDirectoryPath = MutantDirectoryPath();

            _fs.Directory.CreateDirectory(mutantDirectoryPath);

            var refer = _visualStudio.GetReferencedAssemblies();//TODO: Use mono.cecil
            foreach (var referenced in refer)
            {
                string destination = Path.Combine(mutantDirectoryPath, Path.GetFileName(referenced));
                _fs.File.Copy(referenced, destination, overwrite: true); //TODO: Remove overwrite?
            }

            var result = new StoredMutantInfo(mutantDirectoryPath);
   
            foreach (AssemblyDefinition assemblyDefinition in mutant.MutationResult.MutatedAssemblies)
            {
                string file = Path.Combine(mutantDirectoryPath, assemblyDefinition.Name.Name + ".dll");
                _fs.File.Delete(file);
                _assemblyReaderWriter.WriteAssembly(assemblyDefinition, file);
                result.AssembliesPaths.Add(file);
            }
            return result;
        }
        public void DeleteMutantFiles(StoredMutantInfo mutant)
        {
            _fs.Directory.Delete(mutant.DirectoryPath, recursive: true);
        }
        /*
        public string SessionsFile
        {
            get
            {
                string path = _visualStudio.GetMutantsRootFolderPath();
                return Path.Combine(path, "mutants.xml");
            }
        }

        

        public IEnumerable<StoredMutantInfo> LoadSessions()
        {
            if (File.Exists(SessionsFile))
            {
                var ser = new XmlSerializer(typeof(List<StoredMutantInfo>));
                using (var file = new StreamReader(SessionsFile))
                {
                    try
                    {
                        return (List<StoredMutantInfo>)ser.Deserialize(file);
                        
                    }
                    catch (InvalidOperationException e)
                    {
                        _log.Error("Invalid mutants file.", e);
                      
                    }


                }
            }
            return new List<StoredMutantInfo>();
        }
       

        public void SaveSettingsFile(IEnumerable<StoredMutantInfo> mutants)
        {
            var ser = new XmlSerializer(typeof(List<StoredMutantInfo>));

            using (var file = new StreamWriter(SessionsFile))
            {
                ser.Serialize(file, mutants.ToList());
            }
        }
        */

    }
}