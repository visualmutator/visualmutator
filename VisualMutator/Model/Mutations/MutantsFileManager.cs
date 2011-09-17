namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Serialization;

    using Mono.Cecil;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.FileSystem;

    using VisualMutator.Infrastructure;

    using log4net;

    public interface IMutantsFileManager
    {
        string MutantDirectoryPath(MutationSession mutant);

        void StoreMutant(MutationSession mutant, IEnumerable<AssemblyDefinition> assemblies);

        string SessionsFile { get; }

        List<MutationSession> LoadSessions();

        void DeleteMutantFiles(MutationSession mutant);

        void SaveSettingsFile(IList<MutationSession> mutants);
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

        public string MutantDirectoryPath(MutationSession mutant)
        {
            string path = _visualStudio.GetMutantsRootFolderPath();
            return Path.Combine(path, mutant.Name + " - "
                + mutant.DateOfCreation.ToString("dd.MM.yy, HH.mm.ss"));

        }

        public void StoreMutant(MutationSession mutant, IEnumerable<AssemblyDefinition> assemblies)
        {
            

            string mutantDirectoryPath = MutantDirectoryPath(mutant);
            _fs.Directory.CreateDirectory(mutantDirectoryPath);
            foreach (AssemblyDefinition assemblyDefinition in assemblies)
            {
                string file = Path.Combine(mutantDirectoryPath, assemblyDefinition.Name.Name + ".dll");
                _assemblyReaderWriter.WriteAssembly(assemblyDefinition, file);
                mutant.Assemblies.Add(file);
            }
            var refer = _visualStudio.GetReferencedAssemblies();
            foreach (var referenced in refer)
            {
                string destination = Path.Combine(mutantDirectoryPath, Path.GetFileName(referenced));
                _fs.File.Copy(referenced, destination, overwrite: true);
            }
        }

        public string SessionsFile
        {
            get
            {
                string path = _visualStudio.GetMutantsRootFolderPath();
                return Path.Combine(path, "mutants.xml");
            }
        }

        

        public List<MutationSession> LoadSessions()
        {
            if (File.Exists(SessionsFile))
            {
                var ser = new XmlSerializer(typeof(List<MutationSession>));
                using (var file = new StreamReader(SessionsFile))
                {
                    try
                    {
                        return (List<MutationSession>)ser.Deserialize(file);
                        
                    }
                    catch (InvalidOperationException e)
                    {
                        _log.Error("Invalid mutants file.", e);
                      
                    }


                }
            }
            return new List<MutationSession>();
        }
        public void DeleteMutantFiles(MutationSession mutant)
        {
            
            string dir = MutantDirectoryPath(mutant);
            _fs.Directory.Delete(dir);


        }

        public void SaveSettingsFile(IList<MutationSession> mutants)
        {
            var ser = new XmlSerializer(typeof(List<MutationSession>));

            using (var file = new StreamWriter(SessionsFile))
            {
                ser.Serialize(file, mutants.ToList());
            }
        }


    }
}