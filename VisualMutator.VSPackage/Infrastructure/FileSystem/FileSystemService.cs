

namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.FileSystem
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using FileUtils;

    public class FileSystemService
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
        
        public FileSystemService(IFile file, IDirectory directory)
        {
            _file = file;
            _directory = directory;
        }
    }
}
