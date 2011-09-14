namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

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