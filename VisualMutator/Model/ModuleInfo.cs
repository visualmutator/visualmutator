namespace VisualMutator.Model
{
    using JetBrains.Annotations;
    using Microsoft.Cci;
    public interface IModuleInfo
    {
        IModule Module
        {
            get; set;
        }

        string Name
        {
            get; }
    }
    public class ModuleInfo : IModuleInfo
    {
        public ModuleInfo(IModule module, string filePath)
        {
            FilePath = filePath;
            Module = module;
        }

        public IModule Module
        {
            get; set;
        }
        public string FilePath
        {
            get; set;
        }
        public string Name
        {
            get
            {
                return Module.Name.Value;
            } 
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
}