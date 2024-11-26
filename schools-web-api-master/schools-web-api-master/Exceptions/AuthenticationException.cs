using System;

namespace schools_web_api.TokenManager.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base($"Authentication error: {message}") { }
    }
}