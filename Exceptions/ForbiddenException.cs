﻿namespace TimeTrackerAPI.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Forbidden") : base(message){ }
    }
}
