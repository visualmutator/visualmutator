namespace VisualMutator.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using log4net;
    using NUnit.Framework;
    using UsefulTools.FileSystem;
    using UsefulTools.Paths;

    public class ProjectFilesClone : IDisposable
    {
        private readonly IFileSystem _fs;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProjectFilesClone(IFileSystem fs, FilePathAbsolute path)
        {
            _fs = fs;
            Assemblies = new List<FilePathAbsolute>();
            Referenced = new List<FilePathAbsolute>();
            ParentPath = path;
        }

        public FilePathAbsolute ParentPath
        {
            get;
            set;
        }

        public List<FilePathAbsolute> Assemblies
        {
            get;
            set;
        }

        public List<FilePathAbsolute> Referenced
        {
            get;
            private set;
        }

        public bool IsIncomplete { get; set; }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_fs.Directory.Exists(ParentPath.Path))
                {
                    try
                    {
                        _log.Debug("Deleting clone directory: " + ParentPath.Path);
                        _fs.Directory.Delete(ParentPath.Path, recursive: true);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        _log.Warn(e);
                    }
                    catch (IOException e)
                    {
                        _log.Warn(e);
                    }
                }
            }
        }
    }
}