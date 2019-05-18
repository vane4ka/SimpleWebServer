using Autofac;
using SimpleWebServer.Configurations;
using SimpleWebServer.Exceptions;
using SimpleWebServer.Middlewares;
using System;
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
        private IContainer container;
        private string controllerNS;
        private string staticFilesPath;

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
            container = configurator.ConfigureDIContainer();
            controllerNS = configurator.SetControllerNamespace();
            staticFilesPath = configurator.SetStaticFilesPath();
        }

        public void Start()
        {
            if (firstMiddleware == null)
            {
                throw new NotConfigureException("Pipeline is empty");
            }
            else if (container == null)
            {
                throw new NotConfigureException("DI container not configure");
            }
            else if (String.IsNullOrEmpty(controllerNS))
            {
                throw new NotConfigureException("Don't set controller namespace");
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
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("controllerNS", controllerNS);
            data.Add("staticFilesPath", staticFilesPath);
            firstMiddleware.Invoke(context, container, data);
        }
    }
}
