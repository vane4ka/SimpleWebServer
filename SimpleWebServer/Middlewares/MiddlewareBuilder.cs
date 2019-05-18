using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SimpleWebServer.Middlewares
{
    public class MiddlewareBuilder
    {
        private Stack<Type> types = new Stack<Type>();

        public MiddlewareBuilder Add<T>() where T : IMiddleware, new()
        {
            types.Push(typeof(T));
            return this;
        }

        public MiddlewareDelegate Build()
        {
            MiddlewareDelegate first = LastInvoke;
            while (types.Count > 0)
            {
                var mw = Activator.CreateInstance(types.Pop()) as IMiddleware;
                mw.SetNext(first);
                first = mw.RequestProcessing;
            }
            return first;
        }

        private void LastInvoke(HttpListenerContext context, Dictionary<string, object> data)
        {
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = 200;
            byte[] bytes = Encoding.Default.GetBytes("<p>no handler</p>");
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.OutputStream.Close();
        }
    }
}
