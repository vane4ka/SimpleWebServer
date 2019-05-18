using SimpleWebServer.Configurations;
using SimpleWebServer.Exceptions;
using SimpleWebServer.Middlewares;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SimpleWebServer.Server
{
    public class WebServer
    {
        private string url;
        private HttpListener listener;
        private MiddlewareDelegate firstMiddleware;

        public WebServer(string url)
        {
            this.url = url;
            listener = new HttpListener();
            listener.Prefixes.Add(url);
        }

        public void Configure<T>() where T : IConfigurator, new()
        {
            IConfigurator configurator = new T();
            firstMiddleware = configurator.ConfigureMiddleware();
        }

        public void Start()
        {
            if (firstMiddleware == null)
            {
                throw new NotConfigureException("Pipeline is empty");
            }
            listener.Start();
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                Task.Run(() => { Process(context); });
            }
        }

        private void Process(HttpListenerContext context)
        {
            firstMiddleware.Invoke(context, new Dictionary<string, object>());
        }
    }
}
