namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;
    using System.Windows.Documents;

    public class OriginalCodebase
    {
        private readonly List<CciModuleSource> _modules;

        public OriginalCodebase(List<CciModuleSource> modules)
        {
            _modules = modules;
        }

        public List<CciModuleSource> Modules
        {
            get { return _modules; }
        }
    }
}