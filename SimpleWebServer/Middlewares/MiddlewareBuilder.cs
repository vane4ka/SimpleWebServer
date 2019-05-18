using System;
using System.Collections.Generic;

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
            MiddlewareDelegate first = null;
            while (types.Count > 0)
            {
                var mw = Activator.CreateInstance(types.Pop()) as IMiddleware;
                mw.SetNext(first);
                first = mw.RequestProcessing;
            }
            return first;
        }
    }
}
