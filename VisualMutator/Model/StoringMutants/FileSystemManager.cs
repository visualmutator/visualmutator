namespace VisualMutator.Model.StoringMutants
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Infrastructure;
    using log4net;
    using NUnit.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;

    public interface IFileSystemManager : IDisposable
    {
        ProjectFilesClone CreateClone(string name);
        void Initialize();
        Task<ProjectFilesClone> CreateCloneAsync(string name);
    }

    public class FileSystemManager : IFileSystemManager
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IHostEnviromentConnection _hostEnviroment;
        private readonly IFileSystem _fs;
        private List<FilePathAbsolute> _originalProjectFiles;
        private IEnumerable<FilePathAbsolute> _referencedFiles;
        private ProjectFilesClone _mainClone;
        private readonly IList<ProjectFilesClone> _clones;

        public FileSystemManager(
            IHostEnviromentConnection hostEnviroment,
            IFileSystem fs)
        {
            _hostEnviroment = hostEnviroment;
            _fs = fs;

            _clones = new List<ProjectFilesClone>();
        }

        ~FileSystemManager()
        {
            Dispose(false);
        }


        public void Initialize()
        {
            _originalProjectFiles = _hostEnviroment.GetProjectAssemblyPaths().ToList();
            _referencedFiles = GetReferencedAssemblyPaths(_originalProjectFiles).Select(s => s.ToFilePathAbs());

            _mainClone = CreateProjectClone(_referencedFiles, _originalProjectFiles, "MainClone").Result;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mainClone.Dispose();
                foreach (var projectFilesClone in _clones)
                {
                    projectFilesClone.Dispose();
                }
            }
        }

        public ProjectFilesClone CreateClone(string name)
        {
            return CreateCloneAsync(name).Result;
        }

        public async Task<ProjectFilesClone> CreateCloneAsync(string name)
        {
            ProjectFilesClone clone = await CreateProjectClone(_mainClone.Referenced, _mainClone.Assemblies, name);
            clone.IsIncomplete |= _mainClone.IsIncomplete;
            return clone;
        }
        private async Task<ProjectFilesClone> CreateProjectClone(IEnumerable<FilePathAbsolute> referencedFiles, 
            IEnumerable<FilePathAbsolute> projectFiles, string name)
        {

            FilePathAbsolute tmp = CreateTmpDir("VisualMutator-" + name + "-");
            var clone = new ProjectFilesClone(tmp, _fs);
            foreach (var referenced in referencedFiles)
            {
                try
                {
                    var destination = (FilePathAbsolute) tmp.AsChild(referenced);
                    await CopyOverwriteAsync(referenced, destination);
                    clone.Referenced.Add(destination);
                }
                catch (Exception e)
                {
                    _log.Warn("Could not copy file : " +e.Message);
                    clone.IsIncomplete = true;
                }
            }
            foreach (var projFile in projectFiles)
            {
                try
                {
                    var destination = (FilePathAbsolute) tmp.AsChild(projFile);
                    await CopyOverwriteAsync(projFile, destination);
                    clone.Assemblies.Add(destination);
                }
                catch (Exception e)
                {
                    _log.Warn("Could not copy file : " + e.Message);
                    clone.IsIncomplete = true;
                }
            }
            _clones.Add(clone);
            return clone;
        }
        private Task<object> CopyOverwriteAsync(FilePath src, FilePath dst)
        {
            using (FileStream sourceStream = File.Open(src.Path, FileMode.Open))
            {
                using (FileStream destinationStream = File.Create(dst.Path))
                {
                    sourceStream.CopyTo(destinationStream);
                    return Task.FromResult(new object());
                }
            }
        }
        private FilePathAbsolute CreateTmpDir(string s)
        {
            string tmpDirectoryPath = Path.Combine(_hostEnviroment.GetTempPath(), s+Path.GetRandomFileName());
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

       
    }
}