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

    public interface IProjectClonesManager : IDisposable
    {
        ProjectFilesClone CreateClone(string name);
        void Initialize();
        Task<ProjectFilesClone> CreateCloneAsync(string name);
    }

    public class ProjectClonesManager : IProjectClonesManager
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IHostEnviromentConnection _hostEnviroment;
        private readonly IFileSystem _fs;
        private List<FilePathAbsolute> _originalProjectFiles;
        private IEnumerable<FilePathAbsolute> _referencedFiles;
        private ProjectFilesClone _mainClone;
        private readonly IList<ProjectFilesClone> _clones;
        private readonly FilesManager _filesManager;

        public ProjectClonesManager(
            IHostEnviromentConnection hostEnviroment,
            FilesManager filesManager,
            IFileSystem fs)
        {
            _hostEnviroment = hostEnviroment;
            _filesManager = filesManager;
            _fs = fs;

            _clones = new List<ProjectFilesClone>();
        }

        ~ProjectClonesManager()
        {
            Dispose(false);
        }

   


        public void Initialize()
        {
            _originalProjectFiles = _hostEnviroment.GetProjectAssemblyPaths().ToList();
            _referencedFiles = GetReferencedAssemblyPaths(_originalProjectFiles).Select(s => s.ToFilePathAbs());

            FilePathAbsolute tmp = CreateTmpDir("VisualMutator-MainClone-");
            _mainClone = _filesManager.CreateProjectClone(_referencedFiles, _originalProjectFiles, tmp).Result;
            _clones.Add(_mainClone);
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
            FilePathAbsolute tmp = CreateTmpDir("VisualMutator-" + name + "-");
            ProjectFilesClone clone = await _filesManager.CreateProjectClone(_mainClone.Referenced, _mainClone.Assemblies, tmp);
            _clones.Add(clone);
            clone.IsIncomplete |= _mainClone.IsIncomplete;
            return clone;
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