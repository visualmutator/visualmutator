namespace CommonUtilityInfrastructure.FileSystem
{
    #region

    using System;
    using System.IO;
    using System.Security.AccessControl;
    using System.Text;

    #endregion

    public class FileService : IFile
    {
        public void AppendAllText(string path, string contents, Encoding encoding)
        {
            File.AppendAllText(path, contents, encoding);
        }

        public void AppendAllText(string path, string contents)
        {
            File.AppendAllText(path, contents);
        }

        public void AppendText(string path)
        {
            File.AppendText(path);
        }

        public void Copy(string sourceFilename, string destFilename, bool overwrite)
        {
            File.Copy(sourceFilename, destFilename, overwrite);
        }

        public void Copy(string sourceFilename, string destFilename)
        {
            File.Copy(sourceFilename, destFilename);
        }

        public FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity security)
        {
            return File.Create(path, bufferSize, options, security);
        }

        public FileStream Create(string path, int bufferSize, FileOptions options)
        {
            return File.Create(path, bufferSize, options);
        }

        public FileStream Create(string path, int bufferSize)
        {
            return File.Create(path, bufferSize);
        }

        public FileStream Create(string path)
        {
            return File.Create(path);
        }

        public StreamWriter CreateText(string path)
        {
            return File.CreateText(path);
        }

        public void Decrypt(string path)
        {
            File.Decrypt(path);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

        public void Encrypt(string path)
        {
            File.Encrypt(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            return File.GetAccessControl(path, includeSections);
        }

        public FileSecurity GetAccessControl(string path)
        {
            return File.GetAccessControl(path);
        }

        public FileAttributes GetAttributes(string path)
        {
            return File.GetAttributes(path);
        }

        public DateTime GetCreationTime(string path)
        {
            return File.GetCreationTime(path);
        }

        public DateTime GetCreationTimeUtc(string path)
        {
            return File.GetCreationTimeUtc(path);
        }

        public DateTime GetLastAccessTime(string path)
        {
            return File.GetLastAccessTime(path);
        }

        public DateTime GetLastAccessTimeUtc(string path)
        {
            return File.GetLastAccessTimeUtc(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public DateTime GetLastWriteTimeUtc(string path)
        {
            return File.GetLastWriteTimeUtc(path);
        }

        public void Move(string sourceFilename, string destFilename)
        {
            File.Move(sourceFilename, destFilename);
        }

        public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return File.Open(path, mode, access, share);
        }

        public FileStream Open(string path, FileMode mode, FileAccess access)
        {
            return File.Open(path, mode, access);
        }

        public FileStream Open(string path, FileMode mode)
        {
            return File.Open(path, mode);
        }

        public FileStream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public StreamReader OpenText(string path)
        {
            return File.OpenText(path);
        }

        public FileStream OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public string[] ReadAllLines(string path, Encoding encoding)
        {
            return File.ReadAllLines(path, encoding);
        }

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void Replace(string sourceFilename, string destFilename, string destBackupFilename,
            bool ignoreMetadataErrors)
        {
            File.Replace(sourceFilename, destFilename, destBackupFilename, ignoreMetadataErrors);
        }

        public void Replace(string sourceFilename, string destFilename, string destBackupFilename)
        {
            File.Replace(sourceFilename, destFilename, destBackupFilename);
        }

        public void SetAccessControl(string path, FileSecurity security)
        {
            File.SetAccessControl(path, security);
        }

        public void SetAttributes(string path, FileAttributes attributes)
        {
            File.SetAttributes(path, attributes);
        }

        public void SetCreationTime(string path, DateTime creationTime)
        {
            File.SetCreationTime(path, creationTime);
        }

        public void SetCreationTimeUtc(string path, DateTime creationTime)
        {
            File.SetCreationTimeUtc(path, creationTime);
        }

        public void SetLastAccessTime(string path, DateTime creationTime)
        {
            File.SetLastAccessTime(path, creationTime);
        }

        public void SetLastAccessTimeUtc(string path, DateTime creationTime)
        {
            File.SetLastAccessTimeUtc(path, creationTime);
        }

        public void SetLastWriteTime(string path, DateTime creationTime)
        {
            File.SetLastWriteTime(path, creationTime);
        }

        public void SetLastWriteTimeUtc(string path, DateTime creationTime)
        {
            File.SetLastWriteTimeUtc(path, creationTime);
        }

        public void WriteAllBytes(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        public void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            File.WriteAllLines(path, contents, encoding);
        }

        public void WriteAllLines(string path, string[] contents)
        {
            File.WriteAllLines(path, contents);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}