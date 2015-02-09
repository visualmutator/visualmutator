namespace VisualMutator.Model
{
    #region

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using CSharpSourceEmitter;
    using Decompilation;
    using Decompilation.PeToText;
    using Exceptions;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using Microsoft.Cci.MutableContracts;
    using StoringMutants;
    using VisualMutator.Infrastructure;
    using Assembly = Microsoft.Cci.MutableCodeModel.Assembly;
    using Module = Microsoft.Cci.MutableCodeModel.Module;
    using SourceEmitter = CSharpSourceEmitter.SourceEmitter;

    #endregion

    public interface ICciModuleSource : IModuleSource
    {
        IModuleInfo AppendFromFile(string filePath);
        MemoryStream WriteToStream(IModuleInfo module);
        void WriteToStream(IModuleInfo module, FileStream stream, string filePath);
        MetadataReaderHost Host
        {
            get;
        }
        List<ModuleInfo> ModulesInfo
        {
            get;
        }
        SourceEmitter GetSourceEmitter(CodeLanguage language, IModule assembly, SourceEmitterOutputString sourceEmitterOutput);
    }

    public class CciModuleSource : IDisposable, ICciModuleSource
    {
        private readonly MetadataReaderHost _host;
        private List<ModuleInfo> _moduleInfoList;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<string, PdbReader> pdbReaders;

        public List<IModuleInfo> Modules
        {
            get
            {
                return _moduleInfoList.Cast<IModuleInfo>().ToList();
            }
        }
        public List<ModuleInfo> ModulesInfo
        {
            get
            {
                return _moduleInfoList;
            }
        }
        public CciModuleSource(MetadataReaderHost host = null)
        {
            pdbReaders = new Dictionary<string, PdbReader>(StringComparer.OrdinalIgnoreCase);
            _host = host ?? new PeReader.DefaultHost();
            _moduleInfoList = new List<ModuleInfo>();
        }
        public CciModuleSource(MetadataReaderHost host, List<ModuleInfo> moduleInfoList) : this(host)
        {
            _moduleInfoList = moduleInfoList;
        }
        public CciModuleSource(ProjectFilesClone filesClone) : this()
        {
            foreach (var assembliesPath in filesClone.Assemblies)
            {
                var sss = new CodeDeepCopier(Host);
                var m = DecompileFile(assembliesPath.Path);
                var copied = sss.Copy(m.Module);
                m.Module = copied;
                _moduleInfoList.Add(m);
            }
        }
        public CciModuleSource(string path) : this()
        {
            //  var sss = new CodeDeepCopier(this.Host);
            var m = DecompileFile(path);
            // var copied = sss.Copy(m.Module);
            //    m.Module = copied;
            _moduleInfoList.Add(m);
        }

        private CciModuleSource(MetadataReaderHost host, IAssembly module) : this(host)
        {
            _moduleInfoList.Add(new ModuleInfo(module));
        }

        public MetadataReaderHost Host
        {
            get
            {
                return _host;
            }
        }

        public ModuleInfo Module
        {
            get
            {
                return (ModuleInfo)Modules.Single();
            }
        }

        public Guid Guid { get; set; }

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



        public SourceEmitter GetSourceEmitter(CodeLanguage lang, IModule module, SourceEmitterOutputString output)
        {
            var moduleInfo = _moduleInfoList.Single(m => m.Module.Name.UniqueKey == module.Name.UniqueKey);
            var reader = moduleInfo.PdbReader;
            return new VisualSourceEmitter(output, _host, reader, noIL: lang == CodeLanguage.CSharp, printCompilerGeneratedMembers: false);
        }

        public bool TryGetPdbReader(IAssembly assembly, out PdbReader reader)
        {
            string pdbFile = Path.ChangeExtension(assembly.Location, "pdb");
            if (!pdbReaders.TryGetValue(pdbFile, out reader))
                pdbReaders[pdbFile] = reader =
                    File.Exists(pdbFile)
                    ? ReadPdb(pdbFile)
                    : null;

            return reader != null;
        }

        private PdbReader ReadPdb(string pdbFile)
        {
            using (var file = File.OpenRead(pdbFile))
            {
                return new PdbReader(file, _host);
            }
        }

        private IAssembly LoadAssemblyFrom(string filePath)
        {
            IAssembly module = _host.LoadUnitFrom(filePath) as IAssembly;
            _host.RegisterAsLatest(module);
            if (module == null || module == Dummy.Module || module == Dummy.Assembly)
            {
                throw new AssemblyReadException(filePath + " is not a PE file containing a CLR module or assembly.");
            }

            PdbReader pdbReader;
            TryGetPdbReader(module, out pdbReader);

            module = new MetadataDeepCopier(_host).Copy(module);
          //  var decompiled = Decompiler.GetCodeModelFromMetadataModel(_host, module, pdbReader,
          //      DecompilerOptions.None);
            return module;
            //  return new CodeDeepCopier(_host, pdbReader).Copy(decompiled);
        }
        public ModuleInfo DecompileFile(string filePath)
        {
            _log.Info("Decompiling file: " + filePath);

            var decompiledModule = LoadAssemblyFrom(filePath);
            PdbReader pdbReader;
            TryGetPdbReader(decompiledModule, out pdbReader);
            // ILocalScopeProvider localScopeProvider = new Decompiler.LocalScopeProvider(pdbReader);
            _log.Info("Decompiling file finished: " + filePath);
            return new ModuleInfo(decompiledModule, filePath)
            {
                PdbReader = pdbReader,
                LocalScopeProvider = pdbReader,
            };
        }

        public IModuleInfo AppendFromFile(string filePath)
        {
            _log.Info("CommonCompilerInfra.AppendFromFile:" + filePath);
            ModuleInfo module = DecompileFile(filePath);
            _moduleInfoList.Add(module);
            return module;
        }


        public IModuleInfo FindModuleInfo(IModule module)
        {
            return _moduleInfoList.First(m => m.Module.Name.Value == module.Name.Value);
        }

        public MemoryStream WriteToStream(IModuleInfo moduleInfo)
        {
            var module = (ModuleInfo)moduleInfo;
            _log.Info("CommonCompilerInfra.WriteToFile:" + module.Name);
            MemoryStream stream = new MemoryStream();

            if (module.PdbReader == null)
            {
                PeWriter.WritePeToStream(module.Module, _host, stream);

            }
            else
            {
                throw new NotImplementedException();
                //                using (var pdbWriter = new PdbWriter(Path.ChangeExtension(filePath, "pdb"), module.PdbReader))
                //                {
                //                    PeWriter.WritePeToStream(module.Module, _host, stream, module.SourceLocationProvider,
                //                        module.LocalScopeProvider, pdbWriter);
                //                }
            }
            stream.Position = 0;

            return stream;
        }

        public void WriteToStream(IModuleInfo moduleInfo, FileStream stream, string filePath)
        {
            var module = (ModuleInfo)moduleInfo;
            if (module.PdbReader == null)
            {
                PeWriter.WritePeToStream(module.Module, _host, stream);
            }
            else
            {

                using (var pdbWriter = new PdbWriter(Path.ChangeExtension(filePath, "pdb"), module.PdbReader))
                {
                    PeWriter.WritePeToStream(module.Module, _host, stream, module.PdbReader,
                        module.PdbReader, pdbWriter);
                }
            }
        }

        public void ReplaceWith(IAssembly newMod)
        {
            var s = _moduleInfoList.SingleOrDefault(m => m.Name == newMod.Name.Value);
            if (s != null)
            {
                s.Module = newMod;
                _moduleInfoList = _moduleInfoList.Where(m => m == s).ToList();
            }
        }
        public void ReplaceWith(List<IAssembly> modules)
        {
            foreach (var moduleInfo in _moduleInfoList)
            {
                moduleInfo.Module = modules.Single(m => m.Name.Value == moduleInfo.Name);
            }
        }
        public CciModuleSource CloneWith(IAssembly newMod)
        {
            var cci = new CciModuleSource(Host, _moduleInfoList);
            cci.ReplaceWith(newMod);
            return cci;
        }
       
        public CodeDeepCopier CreateCopier()
        {
            //ModuleInfo moduleInfo = (ModuleInfo) Modules.Single();
            return new CodeDeepCopier(Host);//, moduleInfo.SourceLocationProvider, moduleInfo.LocalScopeProvider);

        }
        public Assembly Copy(ModuleInfo module)
        {
            return new MetadataDeepCopier(_host).Copy(module.Module);
        }
        public Assembly Decompile(ModuleInfo module)
        {
            return Decompiler.GetCodeModelFromMetadataModel(_host, module.Module, module.PdbReader, DecompilerOptions.None);
        }
    }
}