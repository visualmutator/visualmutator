namespace VisualMutator.Model.StoringMutants
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Infrastructure;
    using log4net;
    using UsefulTools.DependencyInjection;
    using UsefulTools.Paths;

    public class FilesManager
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IFactory<ProjectFilesClone> _clonesFactory;

        public FilesManager(IFactory<ProjectFilesClone> clonesFactory)
        {
            _clonesFactory = clonesFactory;
        }

        public async Task<ProjectFilesClone> CreateProjectClone(IEnumerable<FilePathAbsolute> referencedFiles, 
            IEnumerable<FilePathAbsolute> projectFiles, FilePathAbsolute tmp)
        {


            var clone = _clonesFactory.CreateWithParams(tmp);
            foreach (var referenced in referencedFiles)
            {
                try
                {
                    var destination = (FilePathAbsolute) tmp.AsChild(referenced);
                    await CopyOverwriteAsync(referenced, destination);
                    clone.Referenced.Add(destination);
                }
                catch (Exception e)
                {
                    _log.Warn("Could not copy file : " +e.Message);
                    clone.IsIncomplete = true;
                }
            }
            foreach (var projFile in projectFiles)
            {
                try
                {
                    var destination = (FilePathAbsolute) tmp.AsChild(projFile);
                    await CopyOverwriteAsync(projFile, destination);
                    clone.Assemblies.Add(destination);
                }
                catch (Exception e)
                {
                    _log.Warn("Could not copy file : " + e.Message);
                    clone.IsIncomplete = true;
                }
            }
            return clone;
        }

        public Task CopyOverwriteAsync(FilePath src, FilePath dst)
        {
            using (FileStream sourceStream = File.OpenRead(src.Path))
            {
                using (FileStream destinationStream = File.Create(dst.Path))
                {
                    lock(this)
                    {
                        sourceStream.CopyTo(destinationStream);
                    }
                    return Task.Delay(0);
                }
            }
        }
    }
}