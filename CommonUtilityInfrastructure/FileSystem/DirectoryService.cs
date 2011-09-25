namespace CommonUtilityInfrastructure.FileSystem
{
    using System;
    using System.IO;
    using System.Security.AccessControl;

    public class DirectoryService : IDirectory
    {
        public DirectoryInfo CreateDirectory(string path, DirectorySecurity security)
        {
            return Directory.CreateDirectory(path, security);
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public void Delete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        public void Delete(string path)
        {
            Directory.Delete(path);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public DirectorySecurity GetAccessControl(string path, AccessControlSections sections)
        {
            return Directory.GetAccessControl(path, sections);
        }

        public DirectorySecurity GetAccessControl(string path)
        {
            return Directory.GetAccessControl(path);
        }

        public DateTime GetCreationTime(string path)
        {
            return Directory.GetCreationTime(path);
        }

        public DateTime GetCreationTimeUtc(string path)
        {
            return Directory.GetCreationTimeUtc(path);
        }

        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public string[] GetDirectories(string path, string searchPattern, SearchOption option)
        {
            return Directory.GetDirectories(path, searchPattern, option);
        }

        public string[] GetDirectories(string path, string searchPattern)
        {
            return Directory.GetDirectories(path, searchPattern);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string GetDirectoryRoot(string path)
        {
            return Directory.GetDirectoryRoot(path);
        }

        public string[] GetFiles(string path, string searchPattern, SearchOption option)
        {
            return Directory.GetFiles(path, searchPattern, option);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public string[] GetFileSystemEntries(string path, string searchPattern)
        {
            return Directory.GetFileSystemEntries(path, searchPattern);
        }

        public string[] GetFileSystemEntries(string path)
        {
            return Directory.GetFileSystemEntries(path);
        }

        public DateTime GetLastAccessTime(string path)
        {
            return Directory.GetLastAccessTime(path);
        }

        public DateTime GetLastAccessTimeUtc(string path)
        {
            return Directory.GetLastAccessTimeUtc(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return Directory.GetLastWriteTime(path);
        }

        public DateTime GetLastWriteTimeUtc(string path)
        {
            return Directory.GetLastWriteTimeUtc(path);
        }

        public string[] GetLogicalDrives()
        {
            return Directory.GetLogicalDrives();
        }

        public DirectoryInfo GetParent(string path)
        {
            return Directory.GetParent(path);
        }

        public void Move(string sourceDirName, string destDirName)
        {
            Directory.Move(sourceDirName, destDirName);
        }

        public void SetAccessControl(string path, DirectorySecurity security)
        {
            Directory.SetAccessControl(path, security);
        }

        public void SetCreationTime(string path, DateTime creationTime)
        {
            Directory.SetCreationTime(path, creationTime);
        }

        public void SetCreationTimeUtc(string path, DateTime creationTime)
        {
            Directory.SetCreationTimeUtc(path, creationTime);
        }

        public void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }

        public void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            Directory.SetLastAccessTime(path, lastAccessTime);
        }

        public void SetLastAccessTimeUtc(string path, DateTime lastAccessTime)
        {
            Directory.SetLastAccessTimeUtc(path, lastAccessTime);
        }

        public void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            Directory.SetLastWriteTime(path, lastWriteTime);
        }

        public void SetLastWriteTimeUtc(string path, DateTime lastWriteTime)
        {
            Directory.SetLastWriteTimeUtc(path, lastWriteTime);
        }
    }
}