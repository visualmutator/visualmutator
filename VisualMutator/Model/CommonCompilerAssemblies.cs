namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using CommonUtilityInfrastructure.Paths;
    using CSharpSourceEmitter;
    using Decompilation;
    using Decompilation.PeToText;
    using Exceptions;
    using JetBrains.Annotations;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using log4net;

    using Module = Microsoft.Cci.MutableCodeModel.Module;
    using SourceEmitter = CSharpSourceEmitter.SourceEmitter;

    public interface ICommonCompilerAssemblies
    {
        List<IModule> Modules { get; }
        void Cleanup();
        IModule AppendFromFile(string filePath);
        Module Copy(IModule module);
        void WriteToFile(IModule module, string filePath);
        void WriteToStream(IModule module, Stream stream);
        MetadataReaderHost Host { get; }
        SourceEmitter GetSourceEmitter(CodeLanguage language, IModule assembly, SourceEmitterOutputString sourceEmitterOutput);
        CommonCompilerAssemblies.ModuleInfo FindModuleInfo(IModule module);
        CommonCompilerAssemblies.ModuleInfo DecompileCopy(IModule module);
    }
    public class Sss : MetadataReaderHost
    {
        public Sss(INameTable nameTable, IInternFactory factory, byte pointerSize, IEnumerable<string> searchPaths, bool searchInGAC) : base(nameTable, factory, pointerSize, searchPaths, searchInGAC)
        {
        }

        public override IUnit LoadUnitFrom(string location)
        {
            throw new NotImplementedException();
        }
        
    }
    public class CommonCompilerAssemblies : IDisposable, ICommonCompilerAssemblies
    {
        private readonly MetadataReaderHost _host;
        private readonly List<ModuleInfo> _moduleInfoList;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public List<IModule> Modules
        {
            get { return _moduleInfoList.Select(_ => _.Module).ToList(); }
        }

        public CommonCompilerAssemblies()
        {
            _host = new PeReader.DefaultHost();
            _moduleInfoList = new List<ModuleInfo>();
        }

        public MetadataReaderHost Host
        {
            get { return _host; }
        }

        public void Dispose()
        {
            _host.Dispose();
        }

      
        public void Cleanup()
        {
            foreach (var moduleInfo in _moduleInfoList)
            {
                if (moduleInfo.PdbReader != null) moduleInfo.PdbReader.Dispose();
              
            }
            _moduleInfoList.Clear();
        }

        public SourceEmitter GetSourceEmitter(CodeLanguage lang, IModule module,SourceEmitterOutputString output)
        {
             var reader = FindModuleInfo(module).PdbReader;
          //  SourceEmitterOutputString sourceEmitterOutput = new SourceEmitterOutputString();
             return new VisualSourceEmitter(output, _host, reader, noIL: lang == CodeLanguage.CSharp, printCompilerGeneratedMembers: false);
        }

        public ModuleInfo DecompileFile(string filePath)
        {
            IModule module = _host.LoadUnitFrom(filePath) as IModule;
            if (module == null || module == Dummy.Module || module == Dummy.Assembly)
            {
                throw new AssemblyReadException(filePath + " is not a PE file containing a CLR module or assembly.");
            }

            PdbReader /*?*/ pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
            {
                Stream pdbStream = File.OpenRead(pdbFile);
                pdbReader = new PdbReader(pdbStream, _host);
            }
            Module decompiledModule = Decompiler.GetCodeModelFromMetadataModel(_host, module, pdbReader);
            ISourceLocationProvider sourceLocationProvider = pdbReader;
            ILocalScopeProvider localScopeProvider = new Decompiler.LocalScopeProvider(pdbReader);
            return new ModuleInfo
            {
                Module = decompiledModule,
                PdbReader = pdbReader,
                LocalScopeProvider = localScopeProvider,
                SourceLocationProvider = sourceLocationProvider,
                FilePath = filePath
            };
        }

        public ModuleInfo DecompileCopy(IModule module)
        {
            ModuleInfo info = FindModuleInfo(module);
            var cci = new CommonCompilerAssemblies();
            ModuleInfo moduleCopy = cci.DecompileFile(info.FilePath);
            moduleCopy.SubCci = cci;
            return moduleCopy;
        }

        public IModule AppendFromFile(string filePath)
        {
            _log.Info("CommonCompilerAssemblies.AppendFromFile:" + filePath);
            ModuleInfo module = DecompileFile(filePath);
            
            _moduleInfoList.Add(module);
         /*   int i = 0;
            while (i++ < 10)
            {
                var copy = Copy(decompiledModule);
                WriteToFile(copy, @"D:\PLIKI\" + Path.GetFileName(filePath));
            }
           */
            return module.Module;
        }


        public IModule ReadFromStream(string filePath)
        {
            IModule module = _host.LoadUnitFrom(filePath) as IModule;
            if (module == null || module == Dummy.Module || module == Dummy.Assembly)
            {
                throw new AssemblyReadException(filePath + " is not a PE file containing a CLR module or assembly.");
            }

            PdbReader /*?*/ pdbReader = null;
            /*  string pdbFile = Path.ChangeExtension(module.Location, "pdb");
              if (File.Exists(pdbFile))
              {
                  Stream pdbStream = File.OpenRead(pdbFile);
                  pdbReader = new PdbReader(pdbStream, _host);  
              }*/
            Module decompiledModule = Decompiler.GetCodeModelFromMetadataModel(_host, module, pdbReader);
            ISourceLocationProvider sourceLocationProvider = pdbReader;
            ILocalScopeProvider localScopeProvider = new Decompiler.LocalScopeProvider(pdbReader);
            _moduleInfoList.Add(new ModuleInfo
            {
                Module = decompiledModule,
                PdbReader = pdbReader,
                LocalScopeProvider = localScopeProvider,
                SourceLocationProvider = sourceLocationProvider
            });
            return decompiledModule;
        }


        public ModuleInfo FindModuleInfo(IModule module)
        {
            return _moduleInfoList.First(m => m.Module.Name.Value == module.Name.Value);
        }
        public Module Copy(IModule module)
        {
           // _log.Info("CommonCompilerAssemblies.Module:" + module.Name);
            var info = FindModuleInfo(module);
            var copier = new CodeDeepCopier(_host, info.SourceLocationProvider);
            return copier.Copy(module);
        }
        public Module Copy(ModuleInfo module)
        {

            var copier = new CodeDeepCopier(_host, module.SourceLocationProvider);
            return copier.Copy(module.Module);
        }
        public void WriteToFile(IModule module, string filePath)
        {
            _log.Info("CommonCompilerAssemblies.WriteToFile:" + module.Name);
            var info = FindModuleInfo(module);
            using (FileStream peStream = File.Create(filePath))
            {
                if (info.PdbReader == null)
                {
                    PeWriter.WritePeToStream(module, _host, peStream);
                }
                else
                {
                    using (var pdbWriter = new PdbWriter(Path.ChangeExtension(filePath, "pdb"), info.PdbReader))
                    {
                        PeWriter.WritePeToStream(module, _host, peStream, info.SourceLocationProvider,
                                                 info.LocalScopeProvider, pdbWriter);
                    }
                }
            }
            
        }

     

        public void WriteToStream(IModule module, Stream stream )
        {
            PeWriter.WritePeToStream(module, _host, stream);

        }
       

        #region Nested type: ModuleInfo

        public class ModuleInfo
        {
            public IModule Module { get; set; }
            public string FilePath { get; set; }

            [CanBeNull]
            public PdbReader PdbReader { get; set; }
            [CanBeNull]
            public ILocalScopeProvider LocalScopeProvider { get; set; }
            [CanBeNull]
            public ISourceLocationProvider SourceLocationProvider
            {
                get;
                set;
            }
            [CanBeNull]
            public CommonCompilerAssemblies SubCci { get; set; }
        }

        #endregion
    }
}