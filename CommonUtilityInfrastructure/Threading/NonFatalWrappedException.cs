namespace CommonUtilityInfrastructure.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable]
    public class NonFatalWrappedException : Exception
    {
   

        public NonFatalWrappedException(Exception inner)
            : base("", inner)
        {
        }

        protected NonFatalWrappedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}