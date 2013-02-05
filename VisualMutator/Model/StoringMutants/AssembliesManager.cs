namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using Mono.Cecil;
    using Types;
    using VisualMutator.Model.Mutations.Structure;

    public interface IAssembliesManager
    {
        StoredAssemblies Store(AssembliesProvider assemblies);

        AssembliesProvider Load(StoredAssemblies assemblies);
        AssembliesProvider Load(IList<IModule> assemblies);
        void SessionEnded();
      
    }



    public class AssembliesManager : IAssembliesManager
    {
        private MonoCecilAssemblyManager _monoCecil;
        private ICommonCompilerAssemblies _commonCompiler;
        private readonly CommonServices _services;
        private readonly IAssemblyReaderWriter _assemblyReaderWriter;


        //  private ClearableCacheAssemblyResolver _assemblyResolver;

        public AssembliesManager(ICommonCompilerAssemblies commonCompiler, CommonServices services,
            IAssemblyReaderWriter assemblyReaderWriter)
        {
            _monoCecil = new MonoCecilAssemblyManager();
            _commonCompiler = commonCompiler;
            _services = services;
            _assemblyReaderWriter = assemblyReaderWriter;
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

      

        public AssembliesProvider Load(IList<IModule> assemblies)
        {
            var pro = new AssembliesProvider()
                {
                    Assemblies = new List<AssemblyDefinition>()
                };
            _services.FileSystem.Directory.CreateDirectory(@"C:\VisualMutatorTemp");
            foreach (var module in assemblies)
            {
                
                string file = @"C:\VisualMutatorTemp\" + module.Name.Value;
                _commonCompiler.WriteToFile(module, file);
                var assemblyDefinition = _assemblyReaderWriter.ReadAssembly(file);
                _services.FileSystem.File.Delete(file);
                pro.Assemblies.Add(assemblyDefinition);
            }


            return pro;
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