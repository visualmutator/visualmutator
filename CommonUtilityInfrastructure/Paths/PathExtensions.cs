namespace CommonUtilityInfrastructure.Paths
{
    using System.IO;

    public static class PathExtensions
    {
 
 
        public static FilePathRelative ToFilePathRel(this string str)
        {
            return new FilePathRelative(str);
        }
        public static FilePathAbsolute ToFilePathAbs(this string str)
        {
            return new FilePathAbsolute(str);
        }
        public static DirectoryPathRelative ToDirPathRel(this string str)
        {
            return new DirectoryPathRelative(str);
        }
        public static DirectoryPathAbsolute ToDirPathAbs(this string str)
        {
            return new DirectoryPathAbsolute(str);
        }
        public static FilePathRelative Join(this FilePathRelative path, string str)
        {
            return new FilePathRelative(Path.Combine(path.Path, str));
        }
        public static FilePathAbsolute Join(this FilePathAbsolute path, string str)
        {
            return new FilePathAbsolute(Path.Combine(path.Path, str));
        }

        public static DirectoryPathRelative Join(this DirectoryPathRelative path, string str)
        {
            return new DirectoryPathRelative(Path.Combine(path.Path, str));
        }

        public static DirectoryPathAbsolute Join(this DirectoryPathAbsolute path, string str)
        {
            return new DirectoryPathAbsolute(Path.Combine(path.Path, str));
        }

    }
}