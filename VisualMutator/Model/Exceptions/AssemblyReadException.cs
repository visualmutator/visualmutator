namespace VisualMutator.Model.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable]
    public class AssemblyReadException : Exception
    {

        public AssemblyReadException()
        {
        }

        public AssemblyReadException(string message)
            : base(message)
        {
        }
      
        public AssemblyReadException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected AssemblyReadException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}