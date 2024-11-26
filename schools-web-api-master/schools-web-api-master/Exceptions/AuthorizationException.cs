using System;

namespace schools_web_api.TokenManager.Exceptions
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base($"Authorization error: {message}") { }
    }
}