namespace VisualMutator.Extensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable]
    public class ReferencedAssemblyNotFoundException : Exception
    {
 

        public ReferencedAssemblyNotFoundException()
        {
        }

        public ReferencedAssemblyNotFoundException(string message)
            : base(message)
        {
        }

        public ReferencedAssemblyNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ReferencedAssemblyNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}