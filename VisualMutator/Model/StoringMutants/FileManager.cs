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
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;

    #endregion

    public interface IFileManager
    {


       
        TestEnvironmentInfo InitTestEnvironment(MutationTestingSession currentSession);

        void CleanupTestEnvironment(TestEnvironmentInfo info);

        List<FilePathAbsolute> CopyOriginalFiles(out bool isError);
        void OnTestingCancelled();
        IEnumerable<string> GetReferencedAssemblies(IList<FilePathAbsolute> projects);
    }

 
    public class FileManager : IFileManager
    {
        private readonly IHostEnviromentConnection _hostEnviroment;

    
        private readonly IFileSystem _fs;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);




        public FileManager(
            IHostEnviromentConnection hostEnviroment,

            IFileSystem fs)
        {
            _hostEnviroment = hostEnviroment;

            _fs = fs;
        }


       public List<FilePathAbsolute> CopyOriginalFiles(out bool isError)
       {
           isError = false;
           string tmpDirectoryPath = Path.Combine(_hostEnviroment.GetTempPath(), Path.GetRandomFileName());
           _fs.Directory.CreateDirectory(tmpDirectoryPath);
           var originalFilesList = new List<FilePathAbsolute>();
           foreach (var referenced in GetReferencedAssemblies(_hostEnviroment.GetProjectAssemblyPaths().ToList()).AsStrings())
           {
               try
               {
                   string destination = Path.Combine(tmpDirectoryPath, Path.GetFileName(referenced));
                   _fs.File.Copy(referenced, destination, overwrite: true);
               }
               catch (Exception e)
               {
                   _log.Warn("File load error", e);
                   isError = true;
               }
           }
           foreach (var projFile in _hostEnviroment.GetProjectAssemblyPaths().AsStrings().ToList())
           {
               try
               {
                   string destination = Path.Combine(tmpDirectoryPath, Path.GetFileName(projFile));
                   _fs.File.Copy(projFile, destination, overwrite: true);
                   originalFilesList.Add(destination.ToFilePathAbs());
               }
               catch (Exception e)
               {
                   _log.Warn("File load error", e);
                   isError = true;
               }
           }
           return originalFilesList;
       }

        public void OnTestingCancelled()
        {
         //   prep.OnTestingCancelled();
        }
        public IEnumerable<string> GetReferencedAssemblies(IList<FilePathAbsolute> projects)
        {
            var list = new HashSet<string>(projects.AsStrings());
            foreach (var binDir in projects.Select(p => p.ParentDirectoryPath))
            {
                var files = Directory.EnumerateFiles(binDir.Path, "*.*", SearchOption.AllDirectories)
                        .Where(s => s.EndsWith(".dll") || s.EndsWith(".pdb"))
                        .Where(p => !projects.Contains(p.ToFilePathAbs()));
                list.AddRange(files);
            }
            return list;
        }
        public TestEnvironmentInfo InitTestEnvironment(MutationTestingSession currentSession)
        {

          //  string mutantDirectoryPath = Path.Combine(_hostEnviroment.GetTempPath(), Path.GetRandomFileName());
           // _fs.Directory.CreateDirectory(mutantDirectoryPath);

            bool loadError;
            var originalFilesList = CopyOriginalFiles(out loadError);
            return new TestEnvironmentInfo(originalFilesList.First().ParentDirectoryPath.ToString());
            /*
            var refer = GetReferencedAssemblies(_hostEnviroment.GetProjectAssemblyPaths().ToList());//TODO: Use better way
            foreach (var referenced in refer)
            {
                string destination = Path.Combine(mutantDirectoryPath, Path.GetFileName(referenced));
                _fs.File.Copy(referenced, destination, overwrite: true); //TODO: Remove overwrite?
            }
            if (currentSession != null)
            {
                foreach (string path in currentSession.Choices.MutantsCreationOptions.AdditionalFilesToCopy)
                {
                    string destination = Path.Combine(mutantDirectoryPath, Path.GetFileName(path));
                    _fs.File.Copy(path, destination, overwrite: true); 
                }
            }*/
        //    return new TestEnvironmentInfo(mutantDirectoryPath);
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