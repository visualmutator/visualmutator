namespace VisualMutator.Extensibility
{
    using Microsoft.Cci;

    public class MethodIdentifier : TypeIdentifier
    {
        private readonly string _methodSignature;

        public string MethodSignature
        {
            get { return _methodSignature; }
        }

        public MethodIdentifier(IMethodDefinition method)
            : base(method.ContainingTypeDefinition as INamedTypeDefinition)
        {
            _methodSignature = method.ToString();
        }
        public override string ToString()
        {
            return string.Format("Identifier: Module: {0}, Type: {1}, Method: {2}", ModuleName, TypeName, MethodSignature);
        }
    }
}