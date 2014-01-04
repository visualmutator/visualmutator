namespace VisualMutator.Model.Exceptions
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

    [Serializable]
    public class NoTestsSelectedException : Exception
    {

        public NoTestsSelectedException()
        {
        }

        public NoTestsSelectedException(string message)
            : base(message)
        {
        }
      
        public NoTestsSelectedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NoTestsSelectedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}