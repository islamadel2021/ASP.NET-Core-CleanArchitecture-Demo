using System.Runtime.Serialization;

namespace Exceptions
{
    public class InvalidPersonException : ArgumentException
    {
        public InvalidPersonException()
        {
        }

        public InvalidPersonException(string? message) : base(message)
        {
        }

        public InvalidPersonException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public InvalidPersonException(string? message, string? paramName) : base(message, paramName)
        {
        }

        public InvalidPersonException(string? message, string? paramName, Exception? innerException) : base(message, paramName, innerException)
        {
        }

        protected InvalidPersonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
