namespace VisualMutator.Model.StoringMutants
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Infrastructure;
    using log4net;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;

    public class FileSystemManager
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly IHostEnviromentConnection _hostEnviroment;
        private readonly IFileSystem _fs;
        private List<FilePathAbsolute> _originalProjectFiles;
        private IEnumerable<FilePathAbsolute> _referencedFiles;

        public FileSystemManager(
            IHostEnviromentConnection hostEnviroment,
            IFileSystem fs)
        {
            _hostEnviroment = hostEnviroment;
            _fs = fs;

            _originalProjectFiles = _hostEnviroment.GetProjectAssemblyPaths().ToList();
            _referencedFiles = GetReferencedAssemblyPaths(_originalProjectFiles).Select(s => s.ToFilePathAbs());

            _mainClone = 


        }

        private object CreateProjectClone()
        {
            FilePathAbsolute tmp = CreateTmpDir();
            foreach (var referenced in _referencedFiles)
            {
                try
                {
                    _fs.File.Copy(referenced.Path, tmp.AsChild(referenced).Path, overwrite: true);
                }
                catch (Exception e)
                {
                    _log.Warn("File load error", e);
                    isError = true;
                }
            }
            tmp.AsChild()
        }

        private FilePathAbsolute CreateTmpDir()
        {
            string tmpDirectoryPath = Path.Combine(_hostEnviroment.GetTempPath(), Path.GetRandomFileName());
            _fs.Directory.CreateDirectory(tmpDirectoryPath);
            return new FilePathAbsolute(tmpDirectoryPath);
        }


        private IEnumerable<string> GetReferencedAssemblyPaths(IList<FilePathAbsolute> projects)
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

        public List<FilePathAbsolute> CopyOriginalFiles(out bool isError)
        {
            isError = false;
            
            _fs.Directory.CreateDirectory(tmpDirectoryPath);
            var originalFilesList = new List<FilePathAbsolute>();
            foreach (var referenced in GetReferencedAssemblyPaths(_hostEnviroment.GetProjectAssemblyPaths().ToList()).AsStrings())
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
    }
}