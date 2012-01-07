namespace CommonUtilityInfrastructure.Paths
{
    #region Usings

    using System;
    using System.Diagnostics;

    #endregion

    public enum PathMode
    {
        Absolute = 0,

        Relative = 1
    }

    // immutable
    [DebuggerDisplay(@"{this.GetType().Name}  {m_Path}")]
    public abstract class BasePath
    {
        //
        //  Ctor
        //

        //
        //  Private and immutable states
        //
        private string m_Path;

        public BasePath(string path, bool isAbsolute)
        {
            if (path == null)
            {
                throw new ArgumentNullException("Input path string is null");
            }

            // No check fro empty paths
            if (path.Length == 0)
            {
                throw new ArgumentException(path, "Input path string is empty");
            }

            path = InternalStringHelper.NormalizePath(path);

            string errorReason;
            if (isAbsolute)
            {
                PathHelper.IsValidAbsolutePath(path, out errorReason);
            }
            else
            {
                PathHelper.IsValidRelativePath(path, out errorReason);
            }
            if (errorReason.Length > 0)
            {
                throw new ArgumentException(path, errorReason);
            }

            // Try remove eventual InnerSpecialDir
            if (InternalStringHelper.ContainsInnerSpecialDir(path))
            {
                InternalStringHelper.TryResolveInnerSpecialDir(path, out path, out errorReason);
            }
            Debug.Assert(errorReason.Length == 0); // The error test has already been done in IsValidXXXPath()
            Debug.Assert(path != null && path.Length > 0);

            m_Path = path;
        }

        protected BasePath()
        {
            m_Path = string.Empty;
        }

        public string Path
        {
            get
            {
                return m_Path;
            }
        }

        public abstract bool IsAbsolutePath { get; }

        public abstract bool IsRelativePath { get; }

        public abstract bool IsDirectoryPath { get; }

        public abstract bool IsFilePath { get; }

        //
        //  ToString and explicit conversion to string
        //

        /*public override string ToString() {
         // Here we put a Debug.Assert to be advised of call to ToString() generated automatically by the C# compiler when concatening path with '+'. 
         Debug.Assert(false,"A call to ToString() has been generated. please use the explicit conversion operator");
         return m_Path;
      }*/

        public bool IsEmpty
        {
            get
            {
                return m_Path.Length == 0;
            }
        }

        //
        // ParentDirectoryPath
        //
        public DirectoryPath ParentDirectoryPath
        {
            get
            {
                string parentPath = InternalStringHelper.GetParentDirectory(Path);
                if (IsRelativePath)
                {
                    return new DirectoryPathRelative(parentPath);
                }
                return new DirectoryPathAbsolute(parentPath);
            }
        }

        //
        //  Drive
        //
        protected string DriveProtected
        {
            get
            {
                Debug.Assert(IsAbsolutePath);
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Cannot infer a drive from an empty path");
                }
                // We have confirmation that the dirve is just the first letter
                // of the absolute path in the documentation of the:
                // System.IO.DriveInfo..ctor(driveName)
                // This ctor sends System.ArgumentException if:
                //     The first letter of driveName is not an uppercase or lowercase letter from
                //     'a' to 'z'.
                return m_Path.Substring(0, 1);
            }
        }

        //
        // Comparison
        //
        private bool Equals(BasePath path)
        {
            Debug.Assert(path != null);

            if (path.IsEmpty)
            {
                return IsEmpty;
            }

            if (IsAbsolutePath != path.IsAbsolutePath)
            {
                return false;
            }
            // A FilePath could be equal to a DirectoryPath
            if (IsDirectoryPath != path.IsDirectoryPath)
            {
                return false;
            }
            return string.Compare(m_Path, path.m_Path, true) == 0;
        }

        public override bool Equals(object obj)
        {
            BasePath basePath = obj as BasePath;
            if (!ReferenceEquals(basePath, null))
            {
                // Comparaison du contenu.
                return Equals(basePath);
            }
            return false;
        }

        //
        //  GetHashCode() when path is key in Dictionnary
        //
        public override int GetHashCode()
        {
            return m_Path.ToLower().GetHashCode() +
                   (IsAbsolutePath ? 1231 : 5677) +
                   (IsFilePath ? 1457 : 3461);
        }

        //
        //  Relative/absolute
        //
        protected static string GetPathRelative(DirectoryPathAbsolute pathFrom, BasePath pathTo)
        {
            Debug.Assert(pathTo.IsAbsolutePath);
            /*if (pathTo.PathMode == PathMode.Relative) {
            throw new ArgumentException(@"Cannot input a relative path to GetPathRelativeTo().
PathFrom = """ + pathFrom.Path + @"""
PathTo   = """ + pathTo.Path + @"""");
         }*/
            if (string.Compare(pathFrom.DriveProtected, pathTo.DriveProtected, true) != 0)
            {
                throw new ArgumentException(@"Cannot compute relative path from 2 paths that are not on the same drive 
PathFrom = """ + pathFrom.Path + @"""
PathTo   = """ + pathTo.Path + @"""");
            }
            // Only work with Directory 
            if (pathTo.IsFilePath)
            {
                pathTo = pathTo.ParentDirectoryPath;
            }
            return InternalStringHelper.GetPathRelativeTo(pathFrom.Path, pathTo.Path);
        }

        protected static string GetAbsolutePathFrom(DirectoryPathAbsolute pathFrom, BasePath pathTo)
        {
            Debug.Assert(pathTo.IsRelativePath);
            /*if (pathTo.PathMode == PathMode.Absolute) {
            throw new ArgumentException(@"Cannot call GetAbsolutePath() on a path already absolute.
PathFrom = """ + pathFrom.Path + @"""
PathTo   = """ + pathTo.Path + @"""");
         }*/
            // Only work with Directory 
            if (pathTo.IsFilePath)
            {
                pathTo = pathTo.ParentDirectoryPath;
            }
            return InternalStringHelper.GetAbsolutePath(pathFrom.Path, pathTo.Path);
        }

        public static explicit operator string(BasePath path)
        {
            return path.Path;
        }

        public static bool operator ==(BasePath path1, object path2)
        {
            if (ReferenceEquals(path1, null))
            {
                return ReferenceEquals(path2, null);
            }
            return path1.Equals(path2);
        }

        public static bool operator !=(BasePath path1, object path2)
        {
            return !(path1 == path2);
        }
    }
}