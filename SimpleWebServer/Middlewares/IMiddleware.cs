using Autofac;
using System.Collections.Generic;
using System.Net;

namespace SimpleWebServer.Middlewares
{
    public delegate void MiddlewareDelegate(HttpListenerContext context, 
        IContainer container, Dictionary<string, object> data);

    public interface IMiddleware
    {
        void SetNext(MiddlewareDelegate next);
        void RequestProcessing(HttpListenerContext context, IContainer container, Dictionary<string, object> data);
    }
}
