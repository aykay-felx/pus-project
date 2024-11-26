using System;

namespace schools_web_api.TokenManager.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base($"Validation error: {message}") {}
    }
}