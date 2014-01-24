namespace VisualMutator.Model.Mutations
{
    public class ProcessingContext
    {
        public AstDescriptor Descriptor { get; set; } 
        public AstNode Method { get; set; } 
        public AstNode Type { get; set; }
        public string CallTypeName { get; set; }
        public string ModuleName { get; set; }

        public override string ToString()
        {
            return string.Format("CallTypeName: {0}, ModuleName: {1}, Descriptor: {2}", CallTypeName, ModuleName, Descriptor);
        }
    }
}