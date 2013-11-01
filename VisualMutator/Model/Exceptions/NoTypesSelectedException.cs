namespace VisualMutator.Model.Exceptions
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

    [Serializable]
    public class NoTypesSelectedException : Exception
    {

        public NoTypesSelectedException()
        {
        }

        public NoTypesSelectedException(string message)
            : base(message)
        {
        }

        public NoTypesSelectedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NoTypesSelectedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}