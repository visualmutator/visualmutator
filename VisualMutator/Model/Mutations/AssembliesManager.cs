namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    using Mono.Cecil;

    using VisualMutator.Model.Mutations.Structure;

    public interface IAssembliesManager
    {
        StoredAssemblies Store(IList<AssemblyDefinition> assemblies);

        IList<AssemblyDefinition> Load(StoredAssemblies assemblies);

        void SessionEnded();
    }
    public class AssembliesManager : IAssembliesManager
    {
        private ClearableCacheAssemblyResolver _assemblyResolver;

        public AssembliesManager()
        {
            _assemblyResolver = new ClearableCacheAssemblyResolver();
        }

        public StoredAssemblies Store(IList<AssemblyDefinition> assemblies)
        {
            IEnumerable<Stream> memoryStreams = Write(assemblies);

            return new StoredAssemblies(memoryStreams.ToList());
        }

        public IList<AssemblyDefinition> Load(StoredAssemblies assemblies)
        {
            return Read(assemblies.Streams);
        }
        private IEnumerable<Stream> Write(IEnumerable<AssemblyDefinition> assemblies)
        {
            return assemblies.Select(assemblyDefinition =>
            {
                var stream = new MemoryStream();

                assemblyDefinition.Write(stream, new WriterParameters());

                stream.Position = 0;

                var resultStream = new MemoryStream();
                DeflateStream compress = new DeflateStream(resultStream, CompressionMode.Compress);
                stream.CopyTo(compress);


                return resultStream;

            }).ToList();
        }

        private IList<AssemblyDefinition> Read(IEnumerable<Stream> streams)
        {
            return streams.Select(stream =>
            {
                stream.Position = 0;
                

                var resultStream = new MemoryStream();
                DeflateStream decompress = new DeflateStream(stream,
                    CompressionMode.Decompress);

                decompress.CopyTo(resultStream);

                resultStream.Position = 0;
                return AssemblyDefinition.ReadAssembly(resultStream,
                                    new ReaderParameters(ReadingMode.Immediate)
                                    {
                                        AssemblyResolver = _assemblyResolver
                                    });
            }).ToList();
        }


        public void SessionEnded()
        {
            _assemblyResolver.ClearCache();
        }
    }
}