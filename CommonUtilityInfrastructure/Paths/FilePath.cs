namespace CommonUtilityInfrastructure.Paths
{
    #region Usings

    using System;
    using System.Diagnostics;

    #endregion

    public abstract class FilePath : BasePath
    {
        protected FilePath()
        {
        }

        // special for empty
        protected FilePath(string path, bool isAbsolute)
            : base(path, isAbsolute)
        {
            if (!InternalStringHelper.HasParentDir(Path))
            {
                throw new ArgumentException(Path, @"The file path has no parent directory.");
            }
        }

        public override bool IsDirectoryPath
        {
            get
            {
                return false;
            }
        }

        public override bool IsFilePath
        {
            get
            {
                return true;
            }
        }

        //
        //  FileName and extension
        //
        public string FileName
        {
            get
            {
                return InternalStringHelper.GetLastName(Path);
            }
        }

        public string FileNameWithoutExtension
        {
            get
            {
                string fileName = FileName;
                string extension = FileExtension;
                if (extension == null || extension.Length == 0)
                {
                    return fileName;
                }
                Debug.Assert(fileName.Length - extension.Length >= 0);
                return fileName.Substring(0, fileName.Length - extension.Length);
            }
        }

        public string FileExtension
        {
            get
            {
                return InternalStringHelper.GetExtension(Path);
            }
        }

        public bool HasExtension(string extension)
        {
            if (extension == null || extension.Length < 2 || extension[0] != '.')
            {
                throw new ArgumentException(@"The input extension string is """ + extension + @""".
The extension must be a non-null string that begins with a dot", "extension");
            }
            // Ignore case comparison
            return (string.Compare(FileExtension, extension, true) == 0);
        }
    }
}