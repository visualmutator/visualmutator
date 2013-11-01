namespace VisualMutator.Model.Tests.Services
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

    [Serializable]
    public class MsTestException : Exception
    {
       
        public MsTestException()
        {
        }

        public MsTestException(string message)
            : base(message)
        {
        }

        public MsTestException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MsTestException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
   
}