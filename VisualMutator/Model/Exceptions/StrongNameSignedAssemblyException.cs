namespace VisualMutator.Model.Exceptions
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

    [Serializable]
    public class StrongNameSignedAssemblyException : Exception
    {

        public StrongNameSignedAssemblyException()
        {
        }

        public StrongNameSignedAssemblyException(string message)
            : base(message)
        {
        }
      
        public StrongNameSignedAssemblyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected StrongNameSignedAssemblyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}