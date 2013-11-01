namespace VisualMutator.Model.Exceptions
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

    [Serializable]
    public class TestingCancelledException : Exception
    {

        public TestingCancelledException()
        {
        }

        public TestingCancelledException(string message)
            : base(message)
        {
        }

        public TestingCancelledException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected TestingCancelledException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}