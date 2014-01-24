namespace VisualMutator.Model.Mutations
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
        public object Object { get; private set; }

        public override string ToString()
        {
            return string.Format("Context: {0}, Object: {1}", Context, Object);
        }
    }
}