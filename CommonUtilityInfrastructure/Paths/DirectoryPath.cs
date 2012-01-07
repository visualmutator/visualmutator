namespace CommonUtilityInfrastructure.Paths
{
    using System;
    using System.IO;

    public abstract class DirectoryPath : BasePath
    {
        protected DirectoryPath()
        {
        }

        // Special for empty Path
        protected DirectoryPath(string path, bool isAbsolute)
            : base(path, isAbsolute)
        {
            

        }

        public override bool IsDirectoryPath
        {
            get
            {
                return true;
            }
        }

        public override bool IsFilePath
        {
            get
            {
                return false;
            }
        }

        //
        //  DirectoryName
        //
        public string DirectoryName
        {
            get
            {
                return InternalStringHelper.GetLastName(Path);
            }
        }

        public bool HasParentDir
        {
            get
            {
                return InternalStringHelper.HasParentDir(Path);
            }
        }
    }
}