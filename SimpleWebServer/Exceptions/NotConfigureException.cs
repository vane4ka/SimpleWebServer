using System;

namespace SimpleWebServer.Exceptions
{
    public class NotConfigureException : Exception
    {
        public NotConfigureException(string message) : base(message)
        {
        }
    }
}
