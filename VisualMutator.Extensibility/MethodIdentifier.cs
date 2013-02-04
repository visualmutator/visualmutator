namespace VisualMutator.Extensibility
{
    using Microsoft.Cci;

    public class MethodIdentifier : TypeIdentifier
    {
        private readonly string _methodName;

        public string MethodName
        {
            get { return _methodName; }
        }

        public MethodIdentifier(IMethodDefinition method)
            : base(method.ContainingTypeDefinition as INamespaceTypeDefinition)
        {
            _methodName = method.Name.Value;
        }
    }
}