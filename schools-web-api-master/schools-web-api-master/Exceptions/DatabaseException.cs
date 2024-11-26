using System;

namespace schools_web_api.TokenManager.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base($"Database error: {message}") {}
    }
}