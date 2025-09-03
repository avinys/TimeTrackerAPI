namespace TimeTrackerAPI.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message = "Validation erorr") : base(message) { }
    }
}
