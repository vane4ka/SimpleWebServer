using Autofac;
using SimpleWebServer.Middlewares;

namespace SimpleWebServer.Configurations
{
    public interface IConfigurator
    {
        MiddlewareDelegate ConfigureMiddleware();
        IContainer ConfigureDIContainer();
        string SetControllerNamespace();
    }
}
