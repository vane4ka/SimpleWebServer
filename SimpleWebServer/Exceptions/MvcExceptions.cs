using System;

namespace SimpleWebServer.Exceptions
{
    public abstract class MvcException : Exception
    {
        public abstract int StatusCode { get; }
    }

    public class BadRequestException : MvcException
    {
        public override int StatusCode { get { return 400; } }
    }

    public class UnauthorizedException : MvcException
    {
        public override int StatusCode { get { return 401; } }
    }

    public class NotFoundException : MvcException
    {
        public override int StatusCode { get { return 404; } }
    }

    public class MethodNotAllowedException : MvcException
    {
        public override int StatusCode { get { return 405; } }
    }
}
