namespace VisualMutator.Model
{
    using System.IO;
    using Mono.Cecil;
    using Mutations;
    using StoringMutants;

    public class MonoCecilAssemblyManager
    {
        private ClearableCacheAssemblyResolver _assemblyResolver = new ClearableCacheAssemblyResolver();

        public StoredAssemblies Store(AssembliesProvider assemblies)
        {//TODO: sss
            return null;//new StoredAssemblies(assemblies.Assemblies.Select(Write).ToList());
        }
        public AssembliesProvider Load(StoredAssemblies assemblies)
        {//TODO: sss
            return null;
           // assemblies.Modules.Streams.Select(Read).ToList();
        }
        private byte[] Write(AssemblyDefinition assemblyDefinition)
        {

            var stream = new MemoryStream();

            assemblyDefinition.Write(stream, new WriterParameters());

            return AssembliesManager.Compress(stream.ToArray());
     
        }

        private AssemblyDefinition Read(byte[] stream)
        {


            byte[] bytes = AssembliesManager.Decompress(stream);


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