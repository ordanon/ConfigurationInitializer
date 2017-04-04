using System;
using System.Runtime.Serialization;

namespace ConfigurationUtils
{
    [Serializable]
    public class FieldInitializationFailureException : Exception
    {
        public FieldInitializationFailureException()
        {
        }

        public FieldInitializationFailureException(string message) : base(message)
        {
        }

        public FieldInitializationFailureException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FieldInitializationFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
