using System;

namespace schools_web_api.TokenManager.Exceptions
{
    public class DataException : Exception
    {
        public DataException(string message) : base($"Data error: {message}") { }
    }
}