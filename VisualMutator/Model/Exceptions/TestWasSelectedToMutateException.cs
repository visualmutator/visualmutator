namespace VisualMutator.Model.Exceptions
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

    [Serializable]
    public class TestWasSelectedToMutateException : Exception
    {

        public TestWasSelectedToMutateException()
        {
        }

        public TestWasSelectedToMutateException(string message)
            : base(message)
        {
        }
      
        public TestWasSelectedToMutateException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected TestWasSelectedToMutateException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}