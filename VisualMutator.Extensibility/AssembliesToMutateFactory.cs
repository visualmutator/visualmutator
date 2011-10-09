namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    public class AssembliesToMutateFactory
    {
        private readonly Func<IList<AssemblyDefinition>> _func;

        public AssembliesToMutateFactory(Func<IList<AssemblyDefinition>> func)
        {
            _func = func;
        }

        public IList<AssemblyDefinition> GetNewCopy()
        {
            return _func();
        }
    }
}