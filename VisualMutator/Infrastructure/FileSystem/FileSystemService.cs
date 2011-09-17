

namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.FileSystem
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using FileUtils;
    using FileUtils.Impl;

    public interface IFileSystem
    {
        IFile File { get; }

        IDirectory Directory { get; }
    }

    public class FileSystemService : IFileSystem
    {
        private readonly IFile _file;

        private readonly IDirectory _directory;

        public IFile File
        {
            get
            {
                return _file;
            }
        }

        public IDirectory Directory
        {
            get
            {
                return _directory;
            }
        }
        
        public FileSystemService()
        {
            _file = new FileService();
            _directory = new DirectoryService();
        }
    }
}
