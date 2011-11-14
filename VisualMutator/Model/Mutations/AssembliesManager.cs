namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;

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
          //  IEnumerable<Stream> memoryStreams = Write(assemblies);

            return new StoredAssemblies(assemblies.Select(Write).ToList());
        }

        public IList<AssemblyDefinition> Load(StoredAssemblies assemblies)
        {
            return assemblies.Streams.Select(Read).ToList();
        }

        private byte[] Write(AssemblyDefinition assemblyDefinition)
        {
            
            var stream = new MemoryStream();

            assemblyDefinition.Write(stream, new WriterParameters());

            return Compress(stream.ToArray());
            /* stream.Position = 0;

            var resultStream = new MemoryStream();
            using (DeflateStream compress = new DeflateStream(resultStream, CompressionMode.Compress))
            {
                stream.CopyTo(compress);
            }
            

            Compress

            if (resultStream.Length == 0)
            {
                throw new InvalidOperationException("resultStream.Length == 0");
            }
            return resultStream;
*/

        }

        private static byte[] Compress(byte[] b)
        {
            using (var ms = new MemoryStream())
            {
                using (var ds = new DeflateStream(ms, CompressionMode.Compress))
                {
             
                    ds.Write(b, 0, b.Length);
                }

                return ms.ToArray();
            }
        }
        public static byte[] Decompress(byte[] bytes)
        {
            using (var uncompressed = new MemoryStream())
            using (var compressed = new MemoryStream(bytes))
            using (var ds = new DeflateStream(compressed, CompressionMode.Decompress))
            {
                ds.CopyTo(uncompressed);
                return uncompressed.ToArray();
            }
        }

        private AssemblyDefinition Read(byte[] stream)
        {
            
             /*   stream.Position = 0;


                var resultStream = new MemoryStream();
                using (DeflateStream decompress = new DeflateStream(stream, CompressionMode.Decompress))
                {
                    decompress.CopyTo(resultStream);
                }

                

                if (resultStream.Length == 0)
                {
                    throw new InvalidOperationException("resultStream.Length == 0");
                }
                resultStream.Position = 0;*/


            byte[] bytes = Decompress(stream);


            var param = new ReaderParameters(ReadingMode.Immediate)
            {
                AssemblyResolver = _assemblyResolver
            };
            return AssemblyDefinition.ReadAssembly(new MemoryStream(bytes, 0, bytes.Length), param);
         
        }

        public void SessionEnded()
        {
            _assemblyResolver.ClearCache();
        }
    }
}