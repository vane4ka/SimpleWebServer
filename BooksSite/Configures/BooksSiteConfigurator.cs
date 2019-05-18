using SimpleWebServer.Configurations;
using SimpleWebServer.Middlewares;
using Autofac;
using BooksSite.Services;
using BooksSite.Controllers;
using System.IO;

namespace BooksSite.Configures
{
    public class BooksSiteConfigurator : IConfigurator
    {
        public MiddlewareDelegate ConfigureMiddleware()
        {
            return new MiddlewareBuilder()
                .Add<StaticFilesMiddleware>()
                .Add<MvcMiddleware>()
                .Add<Final404Middleware>()
                .Build();
        }

        public IContainer ConfigureDIContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<BookService>().As<IBookService>();
            builder.RegisterType<BooksController>();
            return builder.Build();
        }

        public string SetControllerNamespace()
        {
            return "BooksSite.Controllers";
        }

        public string SetStaticFilesPath()
        {
            return $@"{Directory.GetCurrentDirectory()}\..\..\wwwroot\";
        }
    }
}
