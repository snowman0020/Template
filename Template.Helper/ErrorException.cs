namespace Template.Helper
{
    [Serializable()]
    public class ErrorException : Exception
    {
        public ErrorException()
        {
            // Add any type-specific logic, and supply the default message.
        }

        public ErrorException(string message) : base(message)
        {
            // Add any type-specific logic.
        }

        public ErrorException(string message, Exception innerException) :
           base(message, innerException)
        {
            // Add any type-specific logic for inner exceptions.
        }

        //protected ErrorException(SerializationInfo info,
        //   StreamingContext context) : base(info, context)
        //{
        //    // Implement type-specific serialization constructor logic.
        //}
    }
}