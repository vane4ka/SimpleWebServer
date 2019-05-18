using System.Collections.Generic;
using System.IO;
using System.Net;
using Autofac;

namespace SimpleWebServer.Middlewares
{
    public class StaticFilesMiddleware : IMiddleware
    {
        private MiddlewareDelegate next;

        public void SetNext(MiddlewareDelegate next)
        {
            this.next = next;
        }

        public void RequestProcessing(HttpListenerContext context, IContainer container, Dictionary<string, object> data)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            // path examples: ~/index.html ~/styles.css ~/imgs/logo.png
            string url = request.Url.LocalPath;
            if (Path.HasExtension(url))
            {
                string path = $@"{data["staticFilesPath"]}\{url}";
                if (File.Exists(path))
                {
                    response.StatusCode = 200;
                    response.ContentType = "text/html";
                    using (StreamWriter writer = new StreamWriter(response.OutputStream))
                    {
                        writer.Write(File.ReadAllText(path));
                    }
                }
                else
                {
                    response.StatusCode = 404;
                }
                response.Close();
            }
            else
            {
                next?.Invoke(context, container, data);
            }
        }
    }
}
