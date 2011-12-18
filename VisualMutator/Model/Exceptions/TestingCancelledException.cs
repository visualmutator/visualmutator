namespace VisualMutator.Model.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

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