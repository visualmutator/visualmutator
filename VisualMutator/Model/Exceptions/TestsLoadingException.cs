namespace VisualMutator.Model.Exceptions
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

    [Serializable]
    public class TestsLoadingException : Exception
    {

        public TestsLoadingException()
        {
        }

        public TestsLoadingException(string message)
            : base(message)
        {
        }
      
        public TestsLoadingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected TestsLoadingException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}