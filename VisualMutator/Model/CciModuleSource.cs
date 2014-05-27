namespace VisualMutator.Model
{
    #region

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using CSharpSourceEmitter;
    using Decompilation;
    using Decompilation.PeToText;
    using Exceptions;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using StoringMutants;
    using Assembly = Microsoft.Cci.MutableCodeModel.Assembly;
    using Module = Microsoft.Cci.MutableCodeModel.Module;
    using SourceEmitter = CSharpSourceEmitter.SourceEmitter;

    #endregion

    public interface ICciModuleSource : IModuleSource
    {
        void Cleanup();
        IModuleInfo AppendFromFile(string filePath);
        void WriteToFile(IModuleInfo module, string filePath);
        void WriteToStream(IModuleInfo module, Stream stream);
        MetadataReaderHost Host { get; }
        List<ModuleInfo> ModulesInfo { get; }
        SourceEmitter GetSourceEmitter(CodeLanguage language, IModule assembly, SourceEmitterOutputString sourceEmitterOutput);
        //ModuleInfo FindModuleInfo(IModule module);
    }

    public class CciModuleSource : IDisposable, ICciModuleSource
    {
        private readonly MetadataReaderHost _host;
        private readonly List<ModuleInfo> _moduleInfoList;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public List<IModuleInfo> Modules
        {
            get { return _moduleInfoList.Cast<IModuleInfo>().ToList(); }
        }
        public List<ModuleInfo> ModulesInfo
        {
            get
            {
                return _moduleInfoList;
            }
        }
        public CciModuleSource()
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            foreach (var moduleInfo in _moduleInfoList)
            {
                if (moduleInfo.PdbReader != null)
                {
                    moduleInfo.PdbReader.Dispose();
                }
            }
            _moduleInfoList.Clear();
            _host.Dispose();
        }
        ~CciModuleSource()
        {
            Dispose(false);
        }

        public void Cleanup()
        {
            foreach (var moduleInfo in _moduleInfoList)
            {
                if (moduleInfo.PdbReader != null) moduleInfo.PdbReader.Dispose();
              
            }
            _moduleInfoList.Clear();
        }

        public SourceEmitter GetSourceEmitter(CodeLanguage lang, IModule module, SourceEmitterOutputString output)
        {
            var moduleInfo = _moduleInfoList.Single(m => m.Module.Name.UniqueKey == module.Name.UniqueKey);
            var reader = moduleInfo.PdbReader;
             return new VisualSourceEmitter(output, _host, reader, noIL: lang == CodeLanguage.CSharp, printCompilerGeneratedMembers: false);
        }

        public ModuleInfo DecompileFile(string filePath)
        {
            _log.Info("Decompiling file: " + filePath);
            IModule module = _host.LoadUnitFrom(filePath) as IModule;
            if (module == null || module == Dummy.Module || module == Dummy.Assembly)
            {
                throw new AssemblyReadException(filePath + " is not a PE file containing a CLR module or assembly.");
            }

            PdbReader pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdbx");
            if (File.Exists(pdbFile))
            {
                Stream pdbStream = File.OpenRead(pdbFile);
                pdbReader = new PdbReader(pdbStream, _host);
                pdbStream.Close();
            }
            Module decompiledModule = Decompiler.GetCodeModelFromMetadataModel(_host, module, pdbReader);
            ISourceLocationProvider sourceLocationProvider = pdbReader;
            ILocalScopeProvider localScopeProvider = new Decompiler.LocalScopeProvider(pdbReader);
            return new ModuleInfo(decompiledModule, filePath)
            {
                PdbReader = pdbReader,
                LocalScopeProvider = localScopeProvider,
                SourceLocationProvider = sourceLocationProvider,
            };
        }
      
        public IModuleInfo AppendFromFile(string filePath)
        {
            _log.Info("CommonCompilerInfra.AppendFromFile:" + filePath);
            ModuleInfo module = DecompileFile(filePath);
            lock (_moduleInfoList)
            {
                _moduleInfoList.Add(module);
            }
            return module;
        }


        public IModuleInfo FindModuleInfo(IModule module)
        {
            return _moduleInfoList.First(m => m.Module.Name.Value == module.Name.Value);
        }

        public void WriteToFile(IModuleInfo moduleInfo, string filePath)
        {
            lock (this)
            {
                var module = (ModuleInfo)moduleInfo;
                _log.Info("CommonCompilerInfra.WriteToFile:" + module.Name);
                MemoryStream stream = new MemoryStream();
                using (FileStream peStream = File.Create(filePath))
                {
                    if (module.PdbReader == null)
                    {
                        PeWriter.WritePeToStream(module.Module, _host, peStream);
                    }
                    else
                    {
                        using (var pdbWriter = new PdbWriter(Path.ChangeExtension(filePath, "pdb"), module.PdbReader))
                        {
                            PeWriter.WritePeToStream(module.Module, _host, peStream, module.SourceLocationProvider,
                                module.LocalScopeProvider, pdbWriter);
                        }
                    }
                }
            }
           
//            using (FileStream peStream = File.Create(filePath))
//            {
//                stream.CopyTo(peStream);
//            }
        }

        public void WriteToStream(IModuleInfo module, Stream stream )
        {
            PeWriter.WritePeToStream(module.Module, _host, stream);
        }


        #region Nested type: ModuleInfo

       

        #endregion

       
    }
}