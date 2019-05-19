using System.Net;

namespace SimpleWebServer.Controllers
{
    public class BaseController
    {
        public HttpListenerRequest Request { get; set; }
        public HttpListenerResponse Response { get; set; }
    }
}
