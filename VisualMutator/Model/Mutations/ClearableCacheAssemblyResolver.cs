namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;

    using Mono.Cecil;

    public class ClearableCacheAssemblyResolver : BaseAssemblyResolver
    {
        readonly IDictionary<string, AssemblyDefinition> _cache;

        public ClearableCacheAssemblyResolver()
        {
            _cache = new Dictionary<string, AssemblyDefinition>();
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            AssemblyDefinition assembly;
            if (_cache.TryGetValue(name.FullName, out assembly))
            {
                return assembly;
            }

            assembly = base.Resolve(name);
            _cache[name.FullName] = assembly;

            return assembly;
        }

        public void ClearCache()
        {
            _cache.Clear();
        }

    }
}