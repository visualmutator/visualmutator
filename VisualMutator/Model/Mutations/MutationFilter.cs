namespace VisualMutator.Model.Mutations
{
    using System.Collections;
    using System.Collections.Generic;
    using Extensibility;
    using Microsoft.Cci;

    public class MutationFilter
    {
        private readonly IList<TypeIdentifier> _types;
        private readonly IList<MethodIdentifier> _methods;

        public MutationFilter(IList<TypeIdentifier> types, IList<MethodIdentifier> methods)
        {
            _types = types;
            _methods = methods;
        }


        public static MutationFilter AllowAll()
        {
            return new MutationFilter(new List<TypeIdentifier>(), new List<MethodIdentifier>());
        }
        public bool Matches(object obj)
        {
            var type = obj as INamespaceTypeDefinition;
            if(type != null)
            {
                return _types.Count == 0 || _types.Contains(new TypeIdentifier(type));
            }
            var method = obj as IMethodDefinition;
            if (method != null)
            {
                return _methods.Count == 0 || _methods.Contains(new MethodIdentifier(method));
            }
            return false;
        }
    }
}