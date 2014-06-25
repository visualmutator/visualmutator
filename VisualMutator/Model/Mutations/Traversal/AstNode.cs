namespace VisualMutator.Model.Mutations.Traversal
{
    public class AstNode
    {
        public AstNode(ProcessingContext context, object o)
        {
            Context = context;
            Object = o;
        }

        public ProcessingContext Context
        {
            get;
            private set;
        }
        public object Object { get; set; }

        public override string ToString()
        {
            return string.Format("Context: {0}, Object: {1}", Context, Object);
        }
    }
}