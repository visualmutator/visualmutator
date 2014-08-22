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
    using JetBrains.Annotations;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using StoringMutants;
    using Assembly = Microsoft.Cci.MutableCodeModel.Assembly;
    using Module = Microsoft.Cci.MutableCodeModel.Module;
    using SourceEmitter = CSharpSourceEmitter.SourceEmitter;

    #endregion

    public interface ICciModuleSource
    {
        List<IModuleInfo> Modules
        {
            get;
        }
        void Cleanup();
        IModule AppendFromFile(string filePath);
        void WriteToStream(IModuleInfo moduleInfo, FileStream stream, string filePath);
        void WriteToStream(IModule module, Stream stream);
        MetadataReaderHost Host
        {
            get;
        }
       
        SourceEmitter GetSourceEmitter(CodeLanguage language, IModule assembly, SourceEmitterOutputString sourceEmitterOutput);
        CciModuleSource.ModuleInfo FindModuleInfo(IModule module);
        CciModuleSource.ModuleInfo DecompileCopy(IModule module);
    }

    public class CciModuleSource : IDisposable, ICciModuleSource, IModuleSource
    {
        private readonly MetadataReaderHost _host;
        private readonly List<IModuleInfo> _moduleInfoList;
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<IModuleInfo> Modules
        {
            get
            {
                return _moduleInfoList;
            }
        }
        public CciModuleSource()
        {
            _host = new PeReader.DefaultHost();
            _moduleInfoList = new List<IModuleInfo>();
        }
        public CciModuleSource(string path) : this()
        {
            //  var sss = new CodeDeepCopier(this.Host);
            var m = DecompileFile(path);
            // var copied = sss.Copy(m.Module);
            //    m.Module = copied;
            _moduleInfoList.Add(m);
        }
        public MetadataReaderHost Host
        {
            get
            {
                return _host;
            }
        }

        public IModuleInfo Module { get { return Modules.Single(); } }

        public SourceEmitter GetSourceEmitter(CodeLanguage language, IModule assembly, SourceEmitterOutputString sourceEmitterOutput)
        {
            throw new NotImplementedException();
        }

        public ModuleInfo FindModuleInfo(IModule module)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _host.Dispose();
        }


        public void Cleanup()
        {
            
            _moduleInfoList.Clear();
        }

        public SourceEmitter GetSourceEmitter(CodeLanguage lang, IAssembly module, SourceEmitterOutputString output)
        {
            var reader = FindModuleInfo(module).PdbReader;
            //  SourceEmitterOutputString sourceEmitterOutput = new SourceEmitterOutputString();
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

            PdbReader /*?*/ pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdbx");
            if (File.Exists(pdbFile))
            {
                Stream pdbStream = File.OpenRead(pdbFile);
                pdbReader = new PdbReader(pdbStream, _host);
                pdbStream.Close();
            }
            IModule decompiledModule = Decompiler.GetCodeModelFromMetadataModel(_host, module, pdbReader);
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
            return null;
        }

        public void Append(ModuleInfo info)
        {
            _moduleInfoList.Add(info);
        }
        public IModule AppendFromFile(string filePath)
        {
            _log.Info("CommonCompilerInfra.AppendFromFile:" + filePath);
            ModuleInfo module = DecompileFile(filePath);
            lock (_moduleInfoList)
            {
                _moduleInfoList.Add(module);
            }

            /*   int i = 0;
               while (i++ < 10)
               {
                   var copy = Copy(decompiledModule);
                   WriteToFile(copy, @"D:\PLIKI\" + Path.GetFileName(filePath));
               }
              */
            return module.Module;
        }

      
        public void WriteToFile(IModule module, string filePath)
        {
            throw new NotImplementedException();
        }


        public ModuleInfo FindModuleInfo(IAssembly module)
        {
            return (ModuleInfo) _moduleInfoList.First(m => m.Module.Name.Value == module.Name.Value);
        }
      
        public Module Copy(ModuleInfo module)
        {

            var copier = new CodeDeepCopier(_host, module.SourceLocationProvider);
            return copier.Copy(module.Module);
        }
        public void WriteToStream(IModuleInfo moduleInfo, FileStream file, string filePath)
        {
            var module = (ModuleInfo)moduleInfo;
            _log.Info("CommonCompilerInfra.WriteToFile:" + module.Name);
                if (module.PdbReader == null)
                {
                    PeWriter.WritePeToStream(module.Module, _host, file);
                }
                else
                {
                    using (var pdbWriter = new PdbWriter(Path.ChangeExtension(filePath, "pdb"), module.PdbReader))
                    {
                        PeWriter.WritePeToStream(module.Module, _host, file, module.SourceLocationProvider,
                                                 module.LocalScopeProvider, pdbWriter);
                    }
                }

        }



        public void WriteToStream(IModule module, Stream stream)
        {
            PeWriter.WritePeToStream(module, _host, stream);

        }

       
        #region Nested type: ModuleInfo

        public class ModuleInfo : IModuleInfo
        {
            public IModule Module
            {
                get; set;
            }

            public string Name { get { return Module.Name.Value; } }

            public string FilePath
            {
                get; set;
            }

            [CanBeNull]
            public PdbReader PdbReader
            {
                get; set;
            }
            [CanBeNull]
            public ILocalScopeProvider LocalScopeProvider
            {
                get; set;
            }
            [CanBeNull]
            public ISourceLocationProvider SourceLocationProvider
            {
                get;
                set;
            }
            [CanBeNull]
            public CciModuleSource SubCci
            {
                get; set;
            }
        }

        #endregion

        public CodeDeepCopier CreateCopier()
        {
            throw new NotImplementedException();
        }

        public void ReplaceWith(IModule module)
        {
            throw new NotImplementedException();
        }

        public CciModuleSource CloneWith(IModule copied3)
        {
            throw new NotImplementedException();
        }
    }
}