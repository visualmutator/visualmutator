using System;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace FileUtils
{
    /// <summary>
    /// Provides methods for the creation, copying, deletion, moving, and opening of files, and aids in the creation of System.IO.FileStream objects.
    /// </summary>
    public interface IFile
    {
        void AppendAllText(string path, string contents, Encoding encoding);
        void AppendAllText(string path, string contents);
        void AppendText(string path);
        void Copy(string sourceFilename, string destFilename, bool overwrite);
        void Copy(string sourceFilename, string destFilename);
        FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity security);
        FileStream Create(string path, int bufferSize, FileOptions options);
        FileStream Create(string path, int bufferSize);
        FileStream Create(string path);
        StreamWriter CreateText(string path);
        void Decrypt(string path);
        void Delete(string path);
        void Encrypt(string path);
        bool Exists(string path);
        FileSecurity GetAccessControl(string path, AccessControlSections includeSections);
        FileSecurity GetAccessControl(string path);
        FileAttributes GetAttributes(string path);
        DateTime GetCreationTime(string path);
        DateTime GetCreationTimeUtc(string path);
        DateTime GetLastAccessTime(string path);
        DateTime GetLastAccessTimeUtc(string path);
        DateTime GetLastWriteTime(string path);
        DateTime GetLastWriteTimeUtc(string path);
        void Move(string sourceFilename, string destFilename);
        FileStream Open(string path, FileMode mode, FileAccess access, FileShare share);
        FileStream Open(string path, FileMode mode, FileAccess access);
        FileStream Open(string path, FileMode mode);
        FileStream OpenRead(string path);
        StreamReader OpenText(string path);
        FileStream OpenWrite(string path);
        byte[] ReadAllBytes(string path);
        string[] ReadAllLines(string path, Encoding encoding);
        string[] ReadAllLines(string path);
        string ReadAllText(string path, Encoding encoding);
        string ReadAllText(string path);
        void Replace(string sourceFilename, string destFilename, string destBackupFilename, bool ignoreMetadataErrors);
        void Replace(string sourceFilename, string destFilename, string destBackupFilename);
        void SetAccessControl(string path, FileSecurity security);
        void SetAttributes(string path, FileAttributes attributes);
        void SetCreationTime(string path, DateTime creationTime);
        void SetCreationTimeUtc(string path, DateTime creationTime);
        void SetLastAccessTime(string path, DateTime creationTime);
        void SetLastAccessTimeUtc(string path, DateTime creationTime);
        void SetLastWriteTime(string path, DateTime creationTime);
        void SetLastWriteTimeUtc(string path, DateTime creationTime);
        void WriteAllBytes(string path, byte[] bytes);
        void WriteAllLines(string path, string[] contents, Encoding encoding);
        void WriteAllLines(string path, string[] contents);
        void WriteAllText(string path, string contents, Encoding encoding);
        void WriteAllText(string path, string contents);
    }
}