namespace CommonUtilityInfrastructure.Paths
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    #endregion

    internal static class InternalStringHelper
    {
        private const string CURRENT_DIR_SINGLEDOT = ".";

        private const string PARENT_DIR_DOUBLEDOT = "..";

        private static char DIR_SEPARATOR_CHAR = Path.DirectorySeparatorChar;

        private static string DIR_SEPARATOR_STRING = Path.DirectorySeparatorChar.ToString();

        //
        //  Valid path and coherent path mode verification
        //

        // What we call InnerSpecialDir is when at least one '.' or '..' directory is after a valid directory
        // For example tehse paths all contains inner special dir
        // C:\..  
        // .\..\Dir2\.\Dir3
        // .\..\..\Dir2\..\Dir3
        internal static bool ContainsInnerSpecialDir(string path)
        {
            // These cases should have been handled by the calling method and cannot be handled
            Debug.Assert(path != null);
            Debug.Assert(path.Length != 0);
            Debug.Assert(path == NormalizePath(path));

            // Analyze if a /./ or a /../ donc come after a valid DirectoryName
            string[] pathDirs = path.Split(DIR_SEPARATOR_CHAR);
            bool bNextDoubleDotParentDirIsInnerSpecial = false;
            bool bNextSingleDotCurrentDirIsInnerSpecial = false;
            foreach (string pathDir in pathDirs)
            {
                if (pathDir == CURRENT_DIR_SINGLEDOT)
                {
                    if (bNextSingleDotCurrentDirIsInnerSpecial)
                    {
                        return true;
                    }
                }
                else if (pathDir == PARENT_DIR_DOUBLEDOT)
                {
                    if (bNextDoubleDotParentDirIsInnerSpecial)
                    {
                        return true;
                    }
                }
                else
                {
                    bNextDoubleDotParentDirIsInnerSpecial = true;
                }
                bNextSingleDotCurrentDirIsInnerSpecial = true;
            }

            return false;
        }

        internal static bool TryResolveInnerSpecialDir(string path, out string pathResolved, out string failureReason)
        {
            // These cases should have been handled by the calling method and cannot be handled
            Debug.Assert(path != null);
            Debug.Assert(path.Length != 0);
            Debug.Assert(path == NormalizePath(path));

            failureReason = string.Empty; // <- failureReason empty by default 
            pathResolved = string.Empty; // <- pathResolved is empty by default

            // TryResolveInnerSpecialDir() is never called without calling first ContainsInnerSpecialDir()
            Debug.Assert(ContainsInnerSpecialDir(path));

            bool bPathIsAbolute = !CanOnlyBeARelativePath(path);

            string[] pathDirs = path.Split(DIR_SEPARATOR_CHAR);
            Debug.Assert(pathDirs.Length > 0);
            Stack<string> dirStack = new Stack<string>();
            bool bNextDoubleDotParentDirIsInnerSpecial = false;
            bool bNextSingleDotCurrentDirIsInnerSpecial = false;
            foreach (string dir in pathDirs)
            {
                if (dir == CURRENT_DIR_SINGLEDOT)
                {
                    if (bNextSingleDotCurrentDirIsInnerSpecial)
                    {
                        // Just ignore InnerSpecial SingleDot
                        continue;
                    }
                    else
                    {
                        dirStack.Push(dir);
                    }
                }
                else if (dir == PARENT_DIR_DOUBLEDOT)
                {
                    if (bNextDoubleDotParentDirIsInnerSpecial)
                    {
                        // This condition can't be reached because of next conditions
                        /*if (dirStack.Count == 0) {
                     failureReason = @"The path {" + path + @"} references a non-existing parent dir \..\, it cannot be resolved";
                     return false;
                  }*/
                        if (bPathIsAbolute && dirStack.Count == 1)
                        {
                            failureReason = @"The path {" + path + @"} references the parent dir \..\ of the root dir {" + pathDirs[0] + "}, it cannot be resolved";
                            return false;
                        }
                        string dirToRemove = dirStack.Peek();
                        if (dirToRemove == CURRENT_DIR_SINGLEDOT)
                        {
                            Debug.Assert(dirStack.Count == 1);
                            failureReason = @"The path {" + path + @"} references the parent dir \..\ of the current root dir .\, it cannot be resolved";
                            return false;
                        }
                        if (dirToRemove == PARENT_DIR_DOUBLEDOT)
                        {
                            failureReason = @"The path {" + path + @"} references the parent dir \..\ of a parent dir \..\, it cannot be resolved";
                            return false;
                        }
                        dirStack.Pop();
                    }
                    else
                    {
                        dirStack.Push(dir);
                    }
                }
                else
                {
                    dirStack.Push(dir);
                    bNextDoubleDotParentDirIsInnerSpecial = true;
                }
                bNextSingleDotCurrentDirIsInnerSpecial = true;
            }

            // Concatenate the dirs
            StringBuilder stringBuilder = new StringBuilder(path.Length);

            // Notice that the dirs are reverse ordered, that's why we use Insert(0,
            foreach (string dir in dirStack)
            {
                stringBuilder.Insert(0, DIR_SEPARATOR_STRING);
                stringBuilder.Insert(0, dir);
            }
            // Remove the last DIR_SEPARATOR
            stringBuilder.Length = stringBuilder.Length - 1;
            pathResolved = stringBuilder.ToString();
            Debug.Assert(pathResolved == NormalizePath(pathResolved));
            return true;
        }

        internal static bool CanOnlyBeARelativePath(string path)
        {
            Debug.Assert(path != null);
            Debug.Assert(path.Length >= 1);
            return path[0] == '.';
        }

        //
        //  Path normalization
        //
        internal static string NormalizePath(string path)
        {
            Debug.Assert(path != null && path.Length > 0);
            path = path.Replace('/', DIR_SEPARATOR_CHAR);

            // EventuallyRemoveConsecutiveDirSeparator
            string consecutiveDirSeparator = DIR_SEPARATOR_STRING + DIR_SEPARATOR_STRING;
            while (path.IndexOf(consecutiveDirSeparator) != -1)
            {
                path = path.Replace(consecutiveDirSeparator, DIR_SEPARATOR_STRING);
            }

            // EventuallyRemoveEndingDirSeparator
            while (true)
            {
                Debug.Assert(path.Length > 0);
                char lastChar = path[path.Length - 1];
                if (lastChar != DIR_SEPARATOR_CHAR)
                {
                    break;
                }
                path = path.Substring(0, path.Length - 1);
            }

            return path;
        }

        internal static bool HasParentDir(string path)
        {
            return path.Contains(DIR_SEPARATOR_STRING);
        }

        internal static string GetParentDirectory(string path)
        {
            if (!HasParentDir(path))
            {
                throw new InvalidOperationException(@"Can't get the parent dir from path """ + path + @"""");
            }
            int index = path.LastIndexOf(DIR_SEPARATOR_CHAR);
            return path.Substring(0, index);
        }

        internal static string GetLastName(string path)
        {
            if (!HasParentDir(path))
            {
                return string.Empty;
            }
            int index = path.LastIndexOf(DIR_SEPARATOR_CHAR);
            Debug.Assert(index != path.Length - 1);
            return path.Substring(index + 1, path.Length - index - 1);
        }

        internal static string GetExtension(string path)
        {
            string fileName = GetLastName(path);
            int index = fileName.LastIndexOf('.');
            if (index == -1)
            {
                return string.Empty;
            }
            if (index == fileName.Length - 1)
            {
                return string.Empty;
            }
            return fileName.Substring(index, fileName.Length - index);
        }

        //
        //  GetPathRelativeTo
        //
        internal static string GetPathRelativeTo(string pathFrom, string pathTo)
        {
            // Don't return .\ but just . to remain compliant
            if (string.Compare(pathFrom, pathTo, true) == 0)
            {
                return CURRENT_DIR_SINGLEDOT;
            }

            List<string> relativeDirs = new List<string>();
            string[] pathFromDirs = pathFrom.Split(DIR_SEPARATOR_CHAR);
            string[] pathToDirs = pathTo.Split(DIR_SEPARATOR_CHAR);
            int length = Math.Min(pathFromDirs.Length, pathToDirs.Length);
            int lastCommonRoot = -1;

            // find common root
            for (int i = 0; i < length; i++)
            {
                if (string.Compare(pathFromDirs[i], pathToDirs[i], true) != 0)
                {
                    break;
                }
                lastCommonRoot = i;
            }

            // The lastCommon root problem is handled by the calling method and cannot be tested
            Debug.Assert(lastCommonRoot != -1);

            // add relative folders in from path
            for (int i = lastCommonRoot + 1; i < pathFromDirs.Length; i++)
            {
                if (pathFromDirs[i].Length > 0)
                {
                    relativeDirs.Add("..");
                }
            }
            if (relativeDirs.Count == 0)
            {
                relativeDirs.Add(CURRENT_DIR_SINGLEDOT);
            }
            // add to folders to path
            for (int i = lastCommonRoot + 1; i < pathToDirs.Length; i++)
            {
                relativeDirs.Add(pathToDirs[i]);
            }

            // create relative path
            string[] relativeParts = new string[relativeDirs.Count];
            relativeDirs.CopyTo(relativeParts);
            string relativePath = string.Join(DIR_SEPARATOR_STRING, relativeParts);
            return relativePath;
        }

        //
        //  GetAbsolutePath
        //
        internal static string GetAbsolutePath(string pathFrom, string pathTo)
        {
            Debug.Assert(pathTo[0] == '.');

            string[] pathFromDirs = pathFrom.Split(DIR_SEPARATOR_CHAR);
            string[] pathToDirs = pathTo.Split(DIR_SEPARATOR_CHAR);

            // Compute nbParentDirToGoBackInPathFrom
            int nbParentDirToGoBackInPathFrom = 0;
            int nbSpecialDirToGoUpInPathTo = 0;
            for (int i = 0; i < pathToDirs.Length; i++)
            {
                if (pathToDirs[i] == PARENT_DIR_DOUBLEDOT)
                {
                    nbParentDirToGoBackInPathFrom++;
                    nbSpecialDirToGoUpInPathTo++;
                }
                else if (pathToDirs[i] == CURRENT_DIR_SINGLEDOT)
                {
                    nbSpecialDirToGoUpInPathTo++;
                }
                else
                {
                    break;
                }
            }

            // check nbParentDirToGoBackInPathFrom is valid
            if (nbParentDirToGoBackInPathFrom >= pathFromDirs.Length)
            {
                throw new ArgumentException(@"Cannot infer pathTo.GetAbsolutePath(pathFrom) because there are too many parent dirs in pathTo:
PathFrom = """ + pathFrom + @"""
PathTo   = """ + pathTo + @"""");
            }

            // Apply nbParentDirToGoBackInPathFrom to extract part from pathFrom
            string[] dirsExtractedFromPathFrom = new string[(pathFromDirs.Length - nbParentDirToGoBackInPathFrom)];
            for (int i = 0; i < pathFromDirs.Length - nbParentDirToGoBackInPathFrom; i++)
            {
                dirsExtractedFromPathFrom[i] = pathFromDirs[i];
            }
            string partExtractedFromPathFrom = string.Join(DIR_SEPARATOR_STRING, dirsExtractedFromPathFrom);

            // Apply nbParentDirToGoBackInPathFrom to extract part from pathTo
            string[] dirsExtractedFromPathTo = new string[(pathToDirs.Length - nbSpecialDirToGoUpInPathTo)];
            for (int i = 0; i < pathToDirs.Length - nbSpecialDirToGoUpInPathTo; i++)
            {
                dirsExtractedFromPathTo[i] = pathToDirs[i + nbSpecialDirToGoUpInPathTo];
            }
            string partExtractedFromPathTo = string.Join(DIR_SEPARATOR_STRING, dirsExtractedFromPathTo);

            // Concatenate the 2 parts extracted from pathFrom and pathTo
            return partExtractedFromPathFrom + DIR_SEPARATOR_STRING + partExtractedFromPathTo;
        }

        //
        //  Uniform Ressource Locator 
        //
        internal static bool IsUniformRessourceLocatorPath(string path)
        {
            string[] urnSchemes = new[]
            {
                "ftp",
                "http",
                "gopher",
                "mailto",
                "news",
                "nntp",
                "telnet",
                "wais",
                "file",
                "prospero"
            };

            foreach (string scheme in urnSchemes)
            {
                if (path.Length > scheme.Length &&
                    string.Compare(path.Substring(0, scheme.Length),
                        scheme, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool BeginsWithASingleLetterDrive(string path)
        {
            if (path.Length < 2)
            {
                return false;
            }
            if (!char.IsLetter(path[0]))
            {
                return false;
            }
            if (path[1] != ':')
            {
                return false;
            }
            return true;
        }
    }
}