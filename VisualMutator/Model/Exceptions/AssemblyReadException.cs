namespace VisualMutator.Model.Exceptions
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

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