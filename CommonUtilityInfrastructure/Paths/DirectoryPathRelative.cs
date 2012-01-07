namespace CommonUtilityInfrastructure.Paths
{
    #region Usings

    using System;

    #endregion

    public sealed class DirectoryPathRelative : DirectoryPath
    {
        private static DirectoryPathRelative s_Empty = new DirectoryPathRelative();

        public DirectoryPathRelative(string path)
            : base(path, false)
        {
        }

        private DirectoryPathRelative()
        {
        }

        public new DirectoryPathRelative ParentDirectoryPath
        {
            get
            {
                string parentPath = InternalStringHelper.GetParentDirectory(Path);
                return new DirectoryPathRelative(parentPath);
            }
        }

        public static DirectoryPathRelative Empty
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
                return false;
            }
        }

        public override bool IsRelativePath
        {
            get
            {
                return true;
            }
        }

        //
        //  Absolute/Relative path conversion
        //
        public DirectoryPathAbsolute GetAbsolutePathFrom(DirectoryPathAbsolute path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (PathHelper.IsEmpty(this) || PathHelper.IsEmpty(path))
            {
                throw new ArgumentException("Cannot compute an absolute path from an empty path.");
            }
            return new DirectoryPathAbsolute(GetAbsolutePathFrom(path, this));
        }

        public bool CanGetAbsolutePathFrom(DirectoryPathAbsolute path)
        {
            try
            {
                GetAbsolutePathFrom(path);
                return true;
            }
            catch
            {
            }
            return false;
        }

        //
        //  Path Browsing facilities
        //

        public FilePathRelative GetBrotherFileWithName(string fileName)
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

        public DirectoryPathRelative GetBrotherDirectoryWithName(string fileName)
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

        public FilePathRelative GetChildFileWithName(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("filename");
            }
            if (fileName.Length == 0)
            {
                throw new ArgumentException("Empty filename not accepted", "filename");
            }
            if (IsEmpty)
            {
                throw new InvalidOperationException("Can't get a child file name from an empty path");
            }
            return new FilePathRelative(Path + System.IO.Path.DirectorySeparatorChar + fileName);
        }

        public DirectoryPathRelative GetChildDirectoryWithName(string directoryName)
        {
            if (directoryName == null)
            {
                throw new ArgumentNullException("directoryName");
            }
            if (directoryName.Length == 0)
            {
                throw new ArgumentException("Empty directoryName not accepted", "directoryName");
            }
            if (IsEmpty)
            {
                throw new InvalidOperationException("Can't get a child directory name from an empty path");
            }
            return new DirectoryPathRelative(Path + System.IO.Path.DirectorySeparatorChar + directoryName);
        }

        //
        //  Empty DirectoryPathRelative
        //
    }
}