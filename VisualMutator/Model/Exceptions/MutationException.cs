namespace VisualMutator.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

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