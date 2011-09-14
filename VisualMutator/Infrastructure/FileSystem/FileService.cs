using System;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace FileUtils.Impl
{
   
    public class FileService : IFile
    {
        public void AppendAllText(string path, string contents, Encoding encoding)
        {
            System.IO.File.AppendAllText(path, contents, encoding);
        }

        public void AppendAllText(string path, string contents)
        {
            System.IO.File.AppendAllText(path, contents);
        }

        public void AppendText(string path)
        {
            System.IO.File.AppendText(path);
        }

        public void Copy(string sourceFilename, string destFilename, bool overwrite)
        {
            System.IO.File.Copy(sourceFilename, destFilename, overwrite);
        }

        public void Copy(string sourceFilename, string destFilename)
        {
            System.IO.File.Copy(sourceFilename, destFilename);
        }

        public FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity security)
        {
            return System.IO.File.Create(path, bufferSize, options, security);
        }

        public FileStream Create(string path, int bufferSize, FileOptions options)
        {
            return System.IO.File.Create(path, bufferSize, options);
        }

        public FileStream Create(string path, int bufferSize)
        {
            return System.IO.File.Create(path, bufferSize);
        }

        public FileStream Create(string path)
        {
            return System.IO.File.Create(path);
        }

        public StreamWriter CreateText(string path)
        {
            return System.IO.File.CreateText(path);
        }

        public void Decrypt(string path)
        {
            System.IO.File.Decrypt(path);
        }

        public void Delete(string path)
        {
            System.IO.File.Delete(path);
        }

        public void Encrypt(string path)
        {
            System.IO.File.Encrypt(path);
        }

        public bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }

        public FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            return System.IO.File.GetAccessControl(path, includeSections);
        }

        public FileSecurity GetAccessControl(string path)
        {
            return System.IO.File.GetAccessControl(path);
        }

        public FileAttributes GetAttributes(string path)
        {
            return System.IO.File.GetAttributes(path);
        }

        public DateTime GetCreationTime(string path)
        {
            return System.IO.File.GetCreationTime(path);
        }

        public DateTime GetCreationTimeUtc(string path)
        {
            return System.IO.File.GetCreationTimeUtc(path);
        }

        public DateTime GetLastAccessTime(string path)
        {
            return System.IO.File.GetLastAccessTime(path);
        }

        public DateTime GetLastAccessTimeUtc(string path)
        {
            return System.IO.File.GetLastAccessTimeUtc(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return System.IO.File.GetLastWriteTime(path);
        }

        public DateTime GetLastWriteTimeUtc(string path)
        {
            return System.IO.File.GetLastWriteTimeUtc(path);
        }

        public void Move(string sourceFilename, string destFilename)
        {
            System.IO.File.Move(sourceFilename, destFilename);
        }

        public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return System.IO.File.Open(path, mode, access, share);
        }

        public FileStream Open(string path, FileMode mode, FileAccess access)
        {
            return System.IO.File.Open(path, mode, access);
        }

        public FileStream Open(string path, FileMode mode)
        {
            return System.IO.File.Open(path, mode);
        }

        public FileStream OpenRead(string path)
        {
            return System.IO.File.OpenRead(path);
        }

        public StreamReader OpenText(string path)
        {
            return System.IO.File.OpenText(path);
        }

        public FileStream OpenWrite(string path)
        {
            return System.IO.File.OpenWrite(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return System.IO.File.ReadAllBytes(path);
        }

        public string[] ReadAllLines(string path, Encoding encoding)
        {
            return System.IO.File.ReadAllLines(path, encoding);
        }

        public string[] ReadAllLines(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            return System.IO.File.ReadAllText(path, encoding);
        }

        public string ReadAllText(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        public void Replace(string sourceFilename, string destFilename, string destBackupFilename, bool ignoreMetadataErrors)
        {
            System.IO.File.Replace(sourceFilename, destFilename, destBackupFilename, ignoreMetadataErrors);
        }

        public void Replace(string sourceFilename, string destFilename, string destBackupFilename)
        {
            System.IO.File.Replace(sourceFilename, destFilename, destBackupFilename);
        }

        public void SetAccessControl(string path, FileSecurity security)
        {
            System.IO.File.SetAccessControl(path, security);
        }

        public void SetAttributes(string path, FileAttributes attributes)
        {
            System.IO.File.SetAttributes(path, attributes);
        }

        public void SetCreationTime(string path, DateTime creationTime)
        {
            System.IO.File.SetCreationTime(path, creationTime);
        }

        public void SetCreationTimeUtc(string path, DateTime creationTime)
        {
            System.IO.File.SetCreationTimeUtc(path, creationTime);
        }

        public void SetLastAccessTime(string path, DateTime creationTime)
        {
            System.IO.File.SetLastAccessTime(path, creationTime);
        }

        public void SetLastAccessTimeUtc(string path, DateTime creationTime)
        {
            System.IO.File.SetLastAccessTimeUtc(path, creationTime);
        }

        public void SetLastWriteTime(string path, DateTime creationTime)
        {
            System.IO.File.SetLastWriteTime(path, creationTime);
        }

        public void SetLastWriteTimeUtc(string path, DateTime creationTime)
        {
            System.IO.File.SetLastWriteTimeUtc(path, creationTime);
        }

        public void WriteAllBytes(string path, byte[] bytes)
        {
            System.IO.File.WriteAllBytes(path, bytes);
        }

        public void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            System.IO.File.WriteAllLines(path, contents, encoding);
        }

        public void WriteAllLines(string path, string[] contents)
        {
            System.IO.File.WriteAllLines(path, contents);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            System.IO.File.WriteAllText(path, contents, encoding);
        }

        public void WriteAllText(string path, string contents)
        {
            System.IO.File.WriteAllText(path, contents);
        }
    }
}