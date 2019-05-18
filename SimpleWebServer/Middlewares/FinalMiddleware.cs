using System.Collections.Generic;
using System.Net;
using Autofac;

namespace SimpleWebServer.Middlewares
{
    public class Final404Middleware : IMiddleware 
    {
        private MiddlewareDelegate next;

        public void SetNext(MiddlewareDelegate next)
        {
            this.next = next;
        }

        public void RequestProcessing(HttpListenerContext context, IContainer container, Dictionary<string, object> data)
        {
            context.Response.StatusCode = 404;
            context.Response.OutputStream.Close();
        }
    }
}
