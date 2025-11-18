namespace Application.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource is not found
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string resourceName, string identifier)
            : base($"{resourceName} with identifier '{identifier}' was not found.") { }
    }
}
