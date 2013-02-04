namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using Microsoft.Cci;
    using Mono.Cecil;

    using VisualMutator.Model.Mutations.Structure;

    public interface IAssembliesManager
    {
        StoredAssemblies Store(AssembliesProvider assemblies);

        AssembliesProvider Load(StoredAssemblies assemblies);
       
        void SessionEnded();
      
    }



    public class AssembliesManager : IAssembliesManager
    {
        private MonoCecilAssemblyManager _monoCecil;
        private CommonCompilerAssemblies _commonCompiler;


      //  private ClearableCacheAssemblyResolver _assemblyResolver;

        public AssembliesManager(CommonCompilerAssemblies commonCompiler)
        {
            _monoCecil = new MonoCecilAssemblyManager();
            _commonCompiler = commonCompiler;
            // _assemblyResolver = new ClearableCacheAssemblyResolver();
        }

        public StoredAssemblies Store(AssembliesProvider assemblies)
        {
          
            return _monoCecil.Store(assemblies);
            
        }
      
      
      

        public AssembliesProvider Load(StoredAssemblies assemblies)
        {
            return _monoCecil.Load(assemblies);
        }



        public static byte[] Compress(byte[] b)
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

        public void SessionEnded()
        {
            _monoCecil.SessionEnded();
           // _commonCompiler.SessionEnded();
        }

        
    }
}