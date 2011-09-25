namespace CommonUtilityInfrastructure.FileSystem
{
    using System;
    using System.IO;
    using System.Security.AccessControl;

    public interface IDirectory
    {
        DirectoryInfo CreateDirectory(string path, DirectorySecurity security);
        DirectoryInfo CreateDirectory(string path);
        void Delete(string path, bool recursive);
        void Delete(string path);
        bool Exists(string path);
        DirectorySecurity GetAccessControl(string path, AccessControlSections sections);
        DirectorySecurity GetAccessControl(string path);
        DateTime GetCreationTime(string path);
        DateTime GetCreationTimeUtc(string path);
        string GetCurrentDirectory();
        string[] GetDirectories(string path, string searchPattern, SearchOption option);
        string[] GetDirectories(string path, string searchPattern);
        string[] GetDirectories(string path);
        string GetDirectoryRoot(string path);
        string[] GetFiles(string path, string searchPattern, SearchOption option);
        string[] GetFiles(string path, string searchPattern);
        string[] GetFiles(string path);
        string[] GetFileSystemEntries(string path, string searchPattern);
        string[] GetFileSystemEntries(string path);
        DateTime GetLastAccessTime(string path);
        DateTime GetLastAccessTimeUtc(string path);
        DateTime GetLastWriteTime(string path);
        DateTime GetLastWriteTimeUtc(string path);
        string[] GetLogicalDrives();
        DirectoryInfo GetParent(string path);
        void Move(string sourceDirName, string destDirName);
        void SetAccessControl(string path, DirectorySecurity security);
        void SetCreationTime(string path, DateTime creationTime);
        void SetCreationTimeUtc(string path, DateTime creationTime);
        void SetCurrentDirectory(string path);
        void SetLastAccessTime(string path, DateTime lastAccessTime);
        void SetLastAccessTimeUtc(string path, DateTime lastAccessTime);
        void SetLastWriteTime(string path, DateTime lastWriteTime);
        void SetLastWriteTimeUtc(string path, DateTime lastWriteTime);
    }
}