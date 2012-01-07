namespace CommonUtilityInfrastructure.Paths
{
    #region Usings

    using System;

    #endregion

    public sealed class FilePathRelative : FilePath
    {
        private static FilePathRelative s_Empty = new FilePathRelative();

        public FilePathRelative(string path)
            : base(path, false)
        {
        }

        private FilePathRelative()
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

        public static FilePathRelative Empty
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
        public FilePathAbsolute GetAbsolutePathFrom(DirectoryPathAbsolute path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            if (PathHelper.IsEmpty(this) || PathHelper.IsEmpty(path))
            {
                throw new ArgumentException("Cannot compute an absolute path from an empty path.");
            }
            string pathAbsolute = GetAbsolutePathFrom(path, this);
            return new FilePathAbsolute(pathAbsolute + System.IO.Path.DirectorySeparatorChar + FileName);
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

        public FilePathRelative ChangeExtension(string newExtension)
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
            return new FilePathRelative(
                ParentDirectoryPath.GetChildFileWithName(
                                                         FileNameWithoutExtension + newExtension).Path);
        }

        //
        //  Empty FilePathRelative
        //
    }
}