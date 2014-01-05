namespace VisualMutator.Model.Mutations
{
    public class ProcessingContext
    {
        public AstDescriptor Descriptor { get; set; } 
        public AstNode Method { get; set; } 
        public AstDescriptor Type { get; set; }
        public string CallTypeName { get; set; }
        public string ModuleName { get; set; }
    }
}