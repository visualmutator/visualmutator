namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;


    public class OperatorCodeRewriter : OperatorCodeRewriterBase, IOperatorCodeRewriter
    {
        public UserMutationTarget MutationTarget { get; set; }

        public INameTable NameTable { get; set; }

        public MetadataReaderHost Host { get; set; }

        public IModule Module { get; set; }
        public IOperatorUtils OperatorUtils
        {
            get;
            set;
        }

        public VisualCodeRewriter Parent { get; set; }

        public MethodDefinition CurrentMethod
        {
            get;
            set;
        }
        public virtual void Initialize()
        {
          
        }
        public void MethodEnter(MethodDefinition method)
        {
            CurrentMethod = method;
        }
        public void MethodExit(MethodDefinition method)
        {
            CurrentMethod = null;
        }
     
    }
}