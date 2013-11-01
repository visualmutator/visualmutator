namespace VisualMutator.Model.Exceptions
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

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