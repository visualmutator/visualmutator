namespace CommonUtilityInfrastructure.Paths
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    public sealed class FilePathAbsolute : FilePath
    {
        private static FilePathAbsolute s_Empty = new FilePathAbsolute();

        public FilePathAbsolute(string path)
            : base(path, true)
        {
        }

        private FilePathAbsolute()
        {
        }

        //
        //  Absolute/Relative path conversion
        //

        //
        //  Path Browsing facilities
        //
        public new DirectoryPathAbsolute ParentDirectoryPath
        {
            get
            {
                string parentPath = InternalStringHelper.GetParentDirectory(Path);
                return new DirectoryPathAbsolute(parentPath);
            }
        }

        //
        //  Operations that requires physical access
        //
        public string Drive
        {
            get
            {
                return DriveProtected;
            }
        }

        public bool Exists
        {
            get
            {
                return File.Exists(Path);
            }
        }

        public FileInfo FileInfo
        {
            get
            {
                if (!Exists)
                {
                    throw new FileNotFoundException(Path);
                }
                return new FileInfo(Path);
            }
        }

        //
        //  Empty FilePathAbsolute
        //

        public static FilePathAbsolute Empty
        {
            get
            {
                return s_Empty;
            }
        }

        public override bool IsAbsolutePath
        {
            get
            {
                return true;
            }
        }

        public override bool IsRelativePath
        {
            get
            {
                return false;
            }
        }

        public FilePathRelative GetPathRelativeFrom(DirectoryPathAbsolute path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (PathHelper.IsEmpty(this) || PathHelper.IsEmpty(path))
            {
                throw new ArgumentException("Cannot compute a relative path from an empty path.");
            }
            string pathRelative = GetPathRelative(path, this);
            return new FilePathRelative(pathRelative + System.IO.Path.DirectorySeparatorChar + FileName);
        }

        public bool CanGetPathRelativeFrom(DirectoryPathAbsolute path)
        {
            try
            {
                GetPathRelativeFrom(path);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public FilePathAbsolute GetBrotherFileWithName(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("filename");
            }
            if (fileName.Length == 0)
            {
                throw new ArgumentException("Can't get brother of an empty file", "filename");
            }
            if (IsEmpty)
            {
                throw new InvalidOperationException("Can't get brother of an empty file");
            }
            return ParentDirectoryPath.GetChildFileWithName(fileName);
        }

        public DirectoryPathAbsolute GetBrotherDirectoryWithName(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("filename");
            }
            if (fileName.Length == 0)
            {
                throw new ArgumentException("Can't get brother of an empty file", "filename");
            }
            if (IsEmpty)
            {
                throw new InvalidOperationException("Can't get brother of an empty file");
            }
            return ParentDirectoryPath.GetChildDirectoryWithName(fileName);
        }

        public FilePathAbsolute ChangeExtension(string newExtension)
        {
            if (newExtension == null)
            {
                throw new ArgumentNullException(newExtension);
            }
            if (newExtension.Length > 0 && newExtension[0] != '.')
            {
                throw new ArgumentException("A file extension must begin with a dot", newExtension);
            }
            if (IsEmpty)
            {
                throw new InvalidOperationException("Cannot change the extension on an empty file");
            }
            return new FilePathAbsolute(
                ParentDirectoryPath.GetChildFileWithName(
                                                         FileNameWithoutExtension + newExtension).Path);
        }
    }
}