namespace VisualMutator.Model.Verification
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class AssemblyVerificationException : Exception
    {


        public AssemblyVerificationException()
        {
        }

        public AssemblyVerificationException(string message)
            : base(message)
        {
        }

        public AssemblyVerificationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected AssemblyVerificationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}