namespace CommonUtilityInfrastructure.Paths
{
    using System.IO;

    public static class PathExtensions
    {
 
 
        public static FilePathRelative ToFilePathRelative(this string collection)
        {
            return new FilePathRelative(collection);
        }
        public static FilePathAbsolute ToFilePathAbsolute(this string collection)
        {
            return new FilePathAbsolute(collection);
        }
        public static DirectoryPathRelative ToDirectoryPathRelative(this string collection)
        {
            return new DirectoryPathRelative(collection);
        }
        public static DirectoryPathAbsolute ToDirectoryPathAbsolute(this string collection)
        {
            return new DirectoryPathAbsolute(collection);
        }
        public static FilePathRelative Concat(this FilePathRelative path, string str)
        {
            return new FilePathRelative(Path.Combine(path.Path, str));
        }
        public static FilePathAbsolute Concat(this FilePathAbsolute path, string str)
        {
            return new FilePathAbsolute(Path.Combine(path.Path, str));
        }

        public static DirectoryPathRelative Concat(this DirectoryPathRelative path, string str)
        {
            return new DirectoryPathRelative(Path.Combine(path.Path, str));
        }

        public static DirectoryPathAbsolute Concat(this DirectoryPathAbsolute path, string str)
        {
            return new DirectoryPathAbsolute(Path.Combine(path.Path, str));
        }

    }
}