namespace CommonUtilityInfrastructure.Paths
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    #endregion

    public static class PathHelper
    {
        #region Path String Validity

        //----------------------------------------
        //
        // Path String Validity
        //
        //----------------------------------------
        public static bool IsValidAbsolutePath(string path, out string reason)
        {
            reason = string.Empty;

            if (path == null)
            {
                reason = "Input path string is null";
                return false;
            }
            if (path.Length == 0)
            {
                reason = "Empty string for path";
                return false;
            }

            if (InternalStringHelper.CanOnlyBeARelativePath(path))
            {
                reason = "A relative path was inputed where an absolute path was expected.";
                return false;
            }

            if (InternalStringHelper.IsUniformRessourceLocatorPath(path))
            {
                reason = "URN (UniformRessourceLocator) paths are not supported.";
                return false;
            }

            if (!InternalStringHelper.BeginsWithASingleLetterDrive(path))
            {
                reason = "The absolute path doesn't begin with a single letter drive followed by ':'.";
                return false;
            }

            path = InternalStringHelper.NormalizePath(path);
            if (InternalStringHelper.ContainsInnerSpecialDir(path))
            {
                string unusedPath;
                if (!InternalStringHelper.TryResolveInnerSpecialDir(path, out unusedPath, out reason))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsValidRelativePath(string path, out string reason)
        {
            reason = string.Empty;
            if (path == null)
            {
                reason = "Input path string is null";
                return false;
            }
            if (path.Length == 0)
            {
                reason = "Empty string for path";
                return false;
            }

            if (!InternalStringHelper.CanOnlyBeARelativePath(path))
            {
                reason = "An absolute path was inputed where a relative path was expected.";
                return false;
            }

            path = InternalStringHelper.NormalizePath(path);
            if (InternalStringHelper.ContainsInnerSpecialDir(path))
            {
                string unusedPath;
                if (!InternalStringHelper.TryResolveInnerSpecialDir(path, out unusedPath, out reason))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsNullOrEmpty(BasePath basePath)
        {
            return basePath == null || IsEmpty(basePath);
        }

        public static bool IsEmpty(BasePath basePath)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException();
            }
            return basePath.Path.Length == 0;
        }

        public static bool DoesPathHasThisPathMode(BasePath basePath, PathMode pathMode)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException();
            }
            return (basePath.IsAbsolutePath && pathMode == PathMode.Absolute) ||
                   (basePath.IsRelativePath && pathMode == PathMode.Relative);
        }

        #endregion Path String Validity

        #region Infer Relative/Absolute path from string

        //---------------------------------------------------
        //
        // Infer Relative/Absolute path from string
        //
        //---------------------------------------------------
        public static DirectoryPath BuildDirectoryPath(string path)
        {
            string unusedReason;
            if (IsValidRelativePath(path, out unusedReason))
            {
                return new DirectoryPathRelative(path);
            }
            return new DirectoryPathAbsolute(path);
        }

        public static FilePath BuildFilePath(string path)
        {
            string unusedReason;
            if (IsValidRelativePath(path, out unusedReason))
            {
                return new FilePathRelative(path);
            }
            return new FilePathAbsolute(path);
        }

        #endregion Infer Relative/Absolute path from string

        // TODOJAVA Remove also .jar extensions
        public static string TryRemoveDllOrExeExtension(string filePath)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return string.Empty;
            }
            filePath = filePath.Trim();
            if (filePath.Length <= 4)
            {
                return filePath;
            }
            string lastFourChars = filePath.Substring(filePath.Length - 4, 4);
            if (string.Compare(lastFourChars, ".dll", true) == 0 ||
                string.Compare(lastFourChars, ".exe", true) == 0)
            {
                return filePath.Substring(0, filePath.Length - 4);
            }
            return filePath;
        }

        #region Try Rebase path

        //----------------------------------------------
        //
        //   Try Rebase path
        //
        //----------------------------------------------
        // originalPath "A:\X1\X2\X3"  validPath "B:\Y1\X1"  result "B:\Y1\X1\X2\X3"   deeperCommonDirName ="X1"
        // originalPath "A:\X1\X2\X3"  validPath "B:\Y1\Y2"  result null               deeperCommonDirName =null
        // originalPath "A:\X1\X2\X3"  validPath "B:\X1\X2"  result "B:\X1\X2\X3"      deeperCommonDirName ="X2
        // originalPath "A:\X1\X2\X3"  validPath "B:\X2\X3"  result "B:\X2\X3"         deeperCommonDirName ="X3"
        // originalPath "A:\X1\X2\X3"  validPath "B:\X2"     result "B:\X2\X3"         deeperCommonDirName ="X2"
        // originalPath "A:\X1\X2\X3"  validPath "B:\X3\X2"  result "B:\X3\X2\X3"      deeperCommonDirName ="X2"
        // originalPath "A:\X1\X2\X3"  validPath "B:\X3\Y1"  result "B:\X3"            deeperCommonDirName ="X3"
        // originalPath "A:\X1\X2\X3"  validPath "A:\"       result null               deeperCommonDirName =null
        // originalPath "A:\X1\X2\X3"  validPath "A:\Y1"     result null               deeperCommonDirName =null
        // Algo: 
        // 1) Find all common dir name between originalPath and validPath
        // 2) If no common dir name then return null, can't guess the path
        // 3) Get the deeper common dir name (deeper in validPath)
        // 4) Return Path(deeperCommonDirName)+ Pathes in originalPath after deeperCommonDirName
        public static bool TryRebasePath(
            DirectoryPathAbsolute originalPath,
            DirectoryPathAbsolute validPath,
            out DirectoryPathAbsolute rebasedPath)
        {
            rebasedPath = DirectoryPathAbsolute.Empty;
            if (IsNullOrEmpty(originalPath))
            {
                return false;
            }
            if (IsNullOrEmpty(validPath))
            {
                return false;
            }
            List<string> originalPathDirs = new List<string>(originalPath.Path.Split(Path.DirectorySeparatorChar));
            List<string> validPathDirs = new List<string>(validPath.Path.Split(Path.DirectorySeparatorChar));

            int indexInValidPathOfDeeperCommonDirName = -1;
            int indexInOriginalPathOfDeeperCommonDirName = -1;

            // We begin at 1 both loop to avoid Driver in pathes
            for (int validPathIndex = 1; validPathIndex < validPathDirs.Count; validPathIndex++)
            {
                for (int originalPathIndex = 1; originalPathIndex < originalPathDirs.Count; originalPathIndex++)
                {
                    if (validPathDirs[validPathIndex].Length > 0 && // Avoid comparison of empty string that can happen if DirectorySeparatorChar at the end of pathes
                        string.Compare(validPathDirs[validPathIndex], originalPathDirs[originalPathIndex], true) == 0)
                    {
                        indexInValidPathOfDeeperCommonDirName = validPathIndex;
                        indexInOriginalPathOfDeeperCommonDirName = originalPathIndex;
                        break;
                    }
                }
            }
            if (indexInValidPathOfDeeperCommonDirName == -1)
            {
                // No common dir name, return null
                return false;
            }

            // Concate Path(deeperCommonDirName)+ Pathes in originalPath after deeperCommonDirName
            Debug.Assert(indexInOriginalPathOfDeeperCommonDirName >= 0);
            List<string> inferedDirNames = validPathDirs.GetRange(0, indexInValidPathOfDeeperCommonDirName);
            inferedDirNames.AddRange(
                                     originalPathDirs.GetRange(indexInOriginalPathOfDeeperCommonDirName, originalPathDirs.Count - indexInOriginalPathOfDeeperCommonDirName));

            string[] arrayInferedDirNames = new string[inferedDirNames.Count];
            inferedDirNames.CopyTo(arrayInferedDirNames);
            string inferedPath = string.Join(Path.DirectorySeparatorChar.ToString(), arrayInferedDirNames);
            rebasedPath = new DirectoryPathAbsolute(inferedPath);
            return true;
        }

        #endregion Try Rebase path
    }
}