namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Documents;
    using UsefulTools.ExtensionMethods;

    public class OriginalCodebase
    {
        private readonly List<CciModuleSource> _modules;
        private readonly List<string> _testAssemblies;

        public OriginalCodebase(List<CciModuleSource> modules, List<string> testAssemblies = null)
        {
            //.Where(cci => !testAssemblies.Select(Path.GetFileNameWithoutExtension).Contains(cci.Module.Name)).ToList()
            _modules = modules;
            _testAssemblies = testAssemblies.ToEmptyIfNull().ToList();
        }

        public List<CciModuleSource> Modules
        {
            get { return _modules; }
        }
        private bool IsTestAssembly(CciModuleSource cci)
        {
            return _testAssemblies.Select(Path.GetFileNameWithoutExtension).Contains(cci.Module.Name);
        }
        public List<CciModuleSource> ModulesToMutate
        {
            get
            {
                return _modules.WhereNot(IsTestAssembly).ToList();
            }
        }
        public List<CciModuleSource> ModulesWithTests
        {
            get
            {
                return _modules.Where(IsTestAssembly).ToList();
            }
        }
    }
}