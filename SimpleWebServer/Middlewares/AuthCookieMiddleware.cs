using System.Collections.Generic;
using System.Net;
using Autofac;

namespace SimpleWebServer.Middlewares
{
    public class AuthCookieMiddleware : IMiddleware
    {
        private MiddlewareDelegate next;

        public void SetNext(MiddlewareDelegate next)
        {
            this.next = next;
        }

        public void RequestProcessing(HttpListenerContext context, IContainer container, Dictionary<string, object> data)
        {
            Cookie cookie = context.Request.Cookies["username"];
            data.Add("isAuth", cookie != null);
            if (cookie != null)
            {
                data.Add("username", cookie.Value);
            }
            next?.Invoke(context, container, data);
        }
    }
}
