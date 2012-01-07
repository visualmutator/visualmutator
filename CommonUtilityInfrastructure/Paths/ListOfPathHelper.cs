namespace CommonUtilityInfrastructure.Paths
{
    #region Usings

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    #endregion

    public static class ListOfPathHelper
    {
        public static bool ListOfPathsEquals<T>(List<T> list1, List<T> list2) where T : BasePath
        {
            if (list1 == null)
            {
                return list2 == null;
            }
            if (list1.Count != list2.Count)
            {
                return false;
            }
            // Duplicate lists to avoid modifying them
            List<BasePath> list1BasePath = DuplicateAndSort(list1);
            List<BasePath> list2BasePath = DuplicateAndSort(list2);
            for (int i = 0; i < list1BasePath.Count; i++)
            {
                if (!list1BasePath[i].Equals(list2BasePath[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static List<BasePath> DuplicateAndSort<T>(List<T> listIn) where T : BasePath
        {
            List<BasePath> listOut = new List<BasePath>(listIn.ConvertAll<BasePath>(delegate(T t) { return t; }));
            listOut.Sort(delegate(BasePath path1, BasePath path2)
            {
                Debug.Assert(path1 != null && path2 != null);
                if (path1.IsAbsolutePath == path2.IsAbsolutePath && path1.IsFilePath == path2.IsFilePath)
                {
                    return string.Compare(path1.Path, path2.Path, true);
                }
                if (path1.IsAbsolutePath && path2.IsRelativePath)
                {
                    return 1;
                }
                if (path2.IsAbsolutePath && path1.IsRelativePath)
                {
                    return -1;
                }
                if (path1.IsFilePath && path2.IsDirectoryPath)
                {
                    return 1;
                }
                Debug.Assert(path2.IsFilePath && path1.IsDirectoryPath);
                return -1;
            });
            return listOut;
        }

        public static bool TryGetCommonRootDirectory(List<DirectoryPathAbsolute> listOfPaths, out DirectoryPathAbsolute commonRootDirectory)
        {
            commonRootDirectory = DirectoryPathAbsolute.Empty;
            if (listOfPaths == null)
            {
                return false;
            }
            if (listOfPaths.Count == 0)
            {
                return false;
            }

            // If the list contains a path null or empty -> no commonRootDirectory
            foreach (DirectoryPathAbsolute path in listOfPaths)
            {
                if (PathHelper.IsNullOrEmpty(path))
                {
                    return false;
                }
            }

            if (listOfPaths.Count == 1)
            {
                commonRootDirectory = listOfPaths[0];
                return true;
            }

            //
            //  Case where all paths are identical
            //
            bool allPathsAreIdentical = true;
            for (int i = 1; i < listOfPaths.Count; i++)
            {
                if (!listOfPaths[0].Equals(listOfPaths[i]))
                {
                    allPathsAreIdentical = false;
                    break;
                }
            }
            if (allPathsAreIdentical)
            {
                commonRootDirectory = listOfPaths[0];
                return true;
            }

            //
            //  Case where some paths are not on the same drive
            //
            for (int i = 1; i < listOfPaths.Count; i++)
            {
                if (string.Compare(listOfPaths[0].Drive, listOfPaths[i].Drive, true) != 0)
                {
                    return false;
                }
            }

            //
            //  Build listOfSplittedPaths
            //
            List<string[]> listOfSplittedPaths = new List<string[]>();
            int maxDeepForRootDir = int.MaxValue;
            foreach (DirectoryPathAbsolute path in listOfPaths)
            {
                string[] pathSplitted = path.Path.Split(new[] { Path.DirectorySeparatorChar });
                if (pathSplitted.Length < maxDeepForRootDir)
                {
                    maxDeepForRootDir = pathSplitted.Length;
                }
                listOfSplittedPaths.Add(pathSplitted);
            }

            //
            // Compute commonRootDirPath from listOfSplittedPaths
            //
            string commonRootDirPath = string.Empty;
            for (int i = 0; i < maxDeepForRootDir; i++)
            {
                string current = listOfSplittedPaths[0][i];
                foreach (string[] pathSplitted in listOfSplittedPaths)
                {
                    if (string.Compare(pathSplitted[i], current, true) != 0)
                    {
                        commonRootDirectory = new DirectoryPathAbsolute(commonRootDirPath);
                        return true;
                    }
                }
                if (i == 0)
                {
                    commonRootDirPath += current;
                }
                else
                {
                    commonRootDirPath += Path.DirectorySeparatorChar + current;
                }
            }
            commonRootDirectory = new DirectoryPathAbsolute(commonRootDirPath);
            return true;
        }

        public static bool Contains<T>(List<T> list, T path) where T : BasePath
        {
            if (list == null || list.Count == 0)
            {
                return false;
            }
            if (path == null)
            {
                return list.FindAll(delegate(T t) { return t == null; }).Count > 0;
            }
            return list.Find(delegate(T t) { return path.Equals(t); }) != null;
        }

        public static void GetListOfUniqueDirsAndUniqueFileNames(
            List<FilePathAbsolute> listOfFilePath,
            out List<DirectoryPathAbsolute> listOfUniqueDirs,
            out List<string> listOfUniqueFileNames)
        {
            listOfUniqueDirs = new List<DirectoryPathAbsolute>();
            listOfUniqueFileNames = new List<string>();

            if (listOfFilePath == null)
            {
                return;
            }

            foreach (FilePathAbsolute filePath in listOfFilePath)
            {
                if (PathHelper.IsNullOrEmpty(filePath))
                {
                    continue;
                }
                DirectoryPathAbsolute dir = filePath.ParentDirectoryPath;
                if (!Contains(listOfUniqueDirs, dir))
                {
                    listOfUniqueDirs.Add(dir);
                }

                string fileName = filePath.FileName;
                Debug.Assert(fileName != null && fileName.Length > 0);
                if (!ListOfStringHelperContainsIgnoreCase(listOfUniqueFileNames, fileName))
                {
                    listOfUniqueFileNames.Add(fileName);
                }
            } // end foreach
        }

        private static bool ListOfStringHelperContainsIgnoreCase(List<string> list, string str)
        {
            foreach (string tmp in list)
            {
                if (string.Compare(str, tmp, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}