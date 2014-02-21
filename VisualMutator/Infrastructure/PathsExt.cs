namespace VisualMutator.Infrastructure
{
    using System.IO;
    using UsefulTools.Paths;

    public static class PathsExt
    {
         public static FilePath AsChild(this FilePath path, FilePath name)
         {
             if (path.IsRelativePath)
             {
                 return new FilePathRelative(Path.Combine(path.Path, Path.GetFileName(name.Path)));
             }
             else
             {
                 return new FilePathAbsolute(Path.Combine(path.Path, Path.GetFileName(name.Path)));
             }
         }
    }
}