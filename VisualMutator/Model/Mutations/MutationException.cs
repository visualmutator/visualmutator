namespace VisualMutator.Model.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable]
    public class MutationException : Exception
    {
      
        public MutationException()
        {
        }

        public MutationException(string message)
            : base(message)
        {
        }

        public MutationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MutationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}